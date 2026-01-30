using BookHub.LoanService.Domain.Entities;
using BookHub.LoanService.Domain.Ports;
using BookHub.Shared.DTOs;

namespace BookHub.LoanService.Application.Services;

public interface ILoanService
{
    Task<IEnumerable<LoanDto>> GetAllLoansAsync(CancellationToken cancellationToken = default);
    Task<LoanDto?> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);
    Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default);
    Task<LoanDto?> ReturnLoanAsync(Guid id, CancellationToken cancellationToken = default);
}

public class LoanService : ILoanService
{
    private readonly ILoanRepository _repository;
    private readonly ICatalogServiceClient _catalogClient;
    private readonly IUserServiceClient _userClient;
    private readonly ILogger<LoanService> _logger;

    public LoanService(
        ILoanRepository repository,
        ICatalogServiceClient catalogClient,
        IUserServiceClient userClient,
        ILogger<LoanService> logger)
    {
        _repository = repository;
        _catalogClient = catalogClient;
        _userClient = userClient;
        _logger = logger;
    }

    public async Task<IEnumerable<LoanDto>> GetAllLoansAsync(CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetAllAsync(cancellationToken);
        return loans.Select(MapToDto);
    }

    public async Task<LoanDto?> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        return loan == null ? null : MapToDto(loan);
    }

    public async Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return loans.Select(MapToDto);
    }

    public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetOverdueLoansAsync(cancellationToken);
        return loans.Select(MapToDto);
    }

    public async Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userClient.GetUserAsync(dto.UserId, cancellationToken) ?? throw new InvalidOperationException("L'utilisateur {"+dto.UserId+"} n'existe pas");
        var book = await _catalogClient.GetBookAsync(dto.BookId, cancellationToken) ?? throw new InvalidOperationException("Le livre {"+dto.BookId+"} n'existe pas");
        var activeLoansCount = await _repository.GetActiveLoansCountByUserAsync(dto.UserId, cancellationToken);
        if (activeLoansCount >= Loan.MaxActiveLoansPerUser)
            throw new InvalidOperationException("L'utilisateur a atteint la limite maximale d'emprunts");


        var activeLoanForBook = await _repository.GetActiveByBookIdAsync(dto.BookId, cancellationToken);
        if (activeLoanForBook is not null)
            throw new InvalidOperationException("Le livre est déjà emprunté");

        var success = await _catalogClient.DecrementAvailabilityAsync(dto.BookId, cancellationToken);
        if (!success)
            throw new InvalidOperationException("Erreur de mise à jour de la disponibilité");

        var loan = Loan.Create(dto.UserId, dto.BookId, book.Title, user.Email);
        await _repository.AddAsync(loan, cancellationToken);


        _logger.LogInformation("Emprunt créé : {LoanId} pour l'utilisateur {UserId}", loan.Id, loan.UserId);


        return MapToDto(loan);
    }

    public async Task<LoanDto?> ReturnLoanAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        if (loan is null)
            return null;

        loan.Return();

        await _repository.UpdateAsync(loan, cancellationToken);

        await _catalogClient.IncrementAvailabilityAsync(loan.BookId, cancellationToken);

        _logger.LogInformation("L'emprunt a été retourné : {LoanId}", loan.Id);

        return MapToDto(loan);
    }

    private static LoanDto MapToDto(Loan loan) => new(
        loan.Id,
        loan.UserId,
        loan.BookId,
        loan.BookTitle,
        loan.UserEmail,
        loan.LoanDate,
        loan.DueDate,
        loan.ReturnDate,
        (Shared.DTOs.LoanStatus)(int)loan.Status,
        loan.IsOverdue ? loan.CalculatePenalty() : loan.PenaltyAmount
    );
}
