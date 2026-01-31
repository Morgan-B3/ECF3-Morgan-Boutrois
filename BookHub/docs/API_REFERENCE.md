# Référence des endpoints des APIs

## CatalogService (/api/books)
- GET /api/books
- GET /api/books/{id}
- POST /api/books
- PUT /api/books/{id}
- DELETE /api/books/{id}

## UserService (/api/users)
- POST /api/users/register
- POST /api/users/login
- GET /api/users
- GET /api/users/{id}
- PUT /api/users/{id}
- DELETE /api/users/{id}

## LoanService (/api/loans)
- POST /api/loans
- GET /api/loans/{id}
- GET /api/loans/user/{userId}
- PUT /api/loans/{id}/return
- GET /api/loans/overdue

## Swagger
Une fois les services démarrés (voir [DEPLOYMENT.md](DEPLOYMENT.md)), la liste complète de ces endpoints peut-être trouvée aux adresses suivantes :
- CatalogService : http://localhost:5001/swagger  
- UserService : http://localhost:5002/swagger  
- LoanService : http://localhost:5003/swagger  