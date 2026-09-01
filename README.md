# Pizza Store API

A simple backend API for managing pizzas and toppings, built with ASP.NET Core 8 Web API and Entity Framework Core.

## Build

Make sure you have the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed. Check with:

```bash
dotnet --version
```

Clone the repo and restore/build the project:

```bash
git clone https://github.com/Insefferable/pete-zuhh.git
cd pete-zuhh/PizzaStore.Api
dotnet restore
dotnet build
```

## Run

From inside the `PizzaStore.Api` folder:

```bash
dotnet run
```

The API will start at:

```
http://localhost:5205
```

The database is a local SQLite file and is created automatically the first time you run the app — no extra setup needed.

## Test

Test the API using [Postman](https://www.postman.com/) or any similar tool. Send requests to `http://localhost:5205`. For POST and PUT requests, open the **Body** tab, choose **raw**, and set the type to **JSON** before pasting the example body.

Start from a clean database so the ids below match:

```bash
cd PizzaStore.Api
rm pizzastore.db
dotnet ef database update
dotnet run
```

### Toppings

| # | Request | Expected |
|---|---|---|
| 1 | `GET /api/toppings` | `200 OK`, empty list |
| 2 | `POST /api/toppings` — `{ "name": "Cheese" }` | `201 Created`, id 1 |
| 3 | `POST /api/toppings` — `{ "name": "Pepperoni" }` | `201 Created`, id 2 |
| 4 | `POST /api/toppings` — `{ "name": "Olives" }` | `201 Created`, id 3 |
| 5 | `POST /api/toppings` — `{ "name": "Cheese" }` (duplicate) | `409 Conflict` |
| 6 | `GET /api/toppings/1` | `200 OK`, Cheese |
| 7 | `GET /api/toppings/999` | `404 Not Found` |
| 8 | `PUT /api/toppings/3` — `{ "name": "Black Olives" }` | `204 No Content` |
| 9 | `PUT /api/toppings/2` — `{ "name": "Cheese" }` (name taken) | `409 Conflict` |

### Pizzas

| # | Request | Expected |
|---|---|---|
| 10 | `GET /api/pizzas` | `200 OK`, empty list |
| 11 | `POST /api/pizzas` — `{ "name": "Classic Pepperoni", "description": "Pepperoni and cheese", "price": 10.99, "toppingIds": [1, 2] }` | `201 Created`, id 1, both toppings included |
| 12 | `POST /api/pizzas` — same name again (duplicate) | `409 Conflict` |
| 13 | `POST /api/pizzas` — `{ "name": "Bad Pizza", "description": "test", "price": 9.99, "toppingIds": [999] }` | `400 Bad Request` |
| 14 | `GET /api/pizzas` | `200 OK`, one pizza, toppings populated |
| 15 | `GET /api/pizzas/1` | `200 OK` |
| 16 | `GET /api/pizzas/999` | `404 Not Found` |
| 17 | `PUT /api/pizzas/1` — `{ "name": "Classic Pepperoni", "description": "Now with extra cheese", "price": 11.99 }` | `204 No Content`, details updated |
| 18 | `PUT /api/pizzas/1/toppings` — `{ "toppingIds": [1] }` | `204 No Content`, only Cheese remains |
| 19 | `PUT /api/pizzas/1/toppings` — `{ "toppingIds": [999] }` | `400 Bad Request` |

### Cascading deletes

| # | Request | Expected |
|---|---|---|
| 20 | `POST /api/pizzas` — `{ "name": "Greek Style", "description": "test", "price": 9.99, "toppingIds": [3] }` | `201 Created`, id 2 |
| 21 | `DELETE /api/toppings/3` | `204 No Content` |
| 22 | `GET /api/pizzas/2` | `200 OK`, `toppings: []` (topping removed, pizza still exists) |
| 23 | `DELETE /api/pizzas/1` | `204 No Content` |
| 24 | `GET /api/pizzas/1` | `404 Not Found` |
| 25 | `GET /api/toppings` | `200 OK`, Cheese and Pepperoni still listed (deleting the pizza didn't touch them) |

### Race condition

| # | Request | Expected |
|---|---|---|
| 26 | `POST /api/toppings` — `{ "name": "Basil" }`, sent twice back to back (two Postman tabs or Runner, no delay) | One request `201 Created`, the other `409 Conflict` |
