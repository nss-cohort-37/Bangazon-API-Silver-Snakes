# Building the Bangazon Platform API

Welcome, new Bangazonians, to the engineering team here at bangazon.com! Our niche platform connects home aquarium enthusiasts to be able to buy and sell products like fish tanks, filters, fish food, pumps, accessories, etc.

Our immediate need for an application is an internal tool that can be used by our customer service team and our human resources team.

Your team has been assigned to build out a .NET Web API that makes each resource in the Bangazon database available to application developers throughout the rest of the engineering department.

1. Products
1. Product types
1. Customers
1. Orders
1. Payment types
1. Employees
1. Computers
1. Training programs
1. Departments

> **Pro tip:** You do not need to make a Controller for the join tables (i.e. EmployeeTraining), because those aren't resources.

The front end team has been given a prototype API to help them implement a React web application. The prototype is built similar to JSON Server and not a actual long-term solution for persisting and retrieving data. We need your team to build a production ready API to replace the prototype.

If you complete all tickets in your backlog, you should have the following endpoints implemented.

Your front end developers are expecting data back from your API that looks exactly like what's listed below. If the properties don't match, the client application will break. **NOTE** Don't worry about any differences in dummy data the prototype uses vs the data in _your_ database. The only thing that is important is that the shape of each resource should match exactly.

To see examples from the prototype API you can use this base URL:

https://bangazon-prototype-api.herokuapp.com/

**🔥🔥 Check in with your lead developer, product owner, or scrum master if you have _any_ questions 🔥🔥**

## Supported URL Endpoints

### Customers

| Description                               | Endpoint                                                                                                                         | Method | Request Body    | Response Body                     |
| ----------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------- | ------ | --------------- | --------------------------------- |
| Get all customers                         | [api/customers](https://bangazon-prototype-api.herokuapp.com/api/customers)                                                      | GET    |                 | Customer Array                    |
| Search for customer                       | [api/customers?q={someSearchTerm}](https://bangazon-prototype-api.herokuapp.com/api/customers?q=es)                              | GET    |                 | Customer Array                    |
| Get customer by Id                        | [api/customers/{id}](https://bangazon-prototype-api.herokuapp.com/api/customers/1575559407787)                                   | GET    |                 | Customer Object                   |
| Get customer and include product listings | [api/customers/{id}?include=products](https://bangazon-prototype-api.herokuapp.com/api/customers/1575559407755?include=products) | GET    |                 | Customer Object w/ Products Array |
| Add a customer                            | api/customers                                                                                                                    | POST   | Customer Object | Customer Object                   |
| Update a customer                         | api/customers/{id}                                                                                                               | PUT    | Customer Object | Customer Object                   |
| Make customer inactive                    | api/customer/{id}                                                                                                                | DELETE |                 |                                   |

### Products

| Description                      | Endpoint                                                                                                                | Method | Request Body   | Response Body  |
| -------------------------------- | ----------------------------------------------------------------------------------------------------------------------- | ------ | -------------- | -------------- |
| Get all products                 | [api/products](https://bangazon-prototype-api.herokuapp.com/api/products)                                               | GET    |                | Product Array  |
| Search products by name          | [api/products?q={searchTerm}](https://bangazon-prototype-api.herokuapp.com/api/products?q=Accord)                       | GET    |                | Product Array  |
| Sort products by most recent     | [api/products?sortBy=recent](https://bangazon-prototype-api.herokuapp.com/api/products?sortBy=recent)                   | GET    |                | Product Array  |
| Sort products by popularity      | [api/products?sortBy=popularity](https://bangazon-prototype-api.herokuapp.com/api/products?sortBy=popularity)           | GET    |                | Product Array  |
| Sort products by least expensive | [api/products?sortBy=price&asc=true](https://bangazon-prototype-api.herokuapp.com/api/products?sortBy=price&asc=true)   | GET    |                | Product Array  |
| Sort products by most expensive  | [api/products?sortBy=price&asc=false](https://bangazon-prototype-api.herokuapp.com/api/products?sortBy=price&asc=false) | GET    |                | Product Array  |
| Add a new product                | api/products                                                                                                            | POST   | Product Object | Product Object |
| Update a product                 | api/products/{id}                                                                                                       | PUT    | Product Object | Product Object |
| Remove a product                 | api/products/{id}                                                                                                       | DELETE |                |                |

### Payment Types

| Description            | Endpoint                                                                                             | Method | Request Body       | Response Body      |
| ---------------------- | ---------------------------------------------------------------------------------------------------- | ------ | ------------------ | ------------------ |
| Get all payment types  | [api/paymentTypes](https://bangazon-prototype-api.herokuapp.com/api/paymentTypes)                    | GET    |                    | PaymentType Array  |
| Get payment type by id | [api/paymentTypes/{id}](https://bangazon-prototype-api.herokuapp.com/api/paymentTypes/1575501974872) | GET    |                    | PaymentType Object |
| Add new payment type   | api/paymentTypes                                                                                     | POST   | PaymentType Object | PaymentType Object |
| Remove a payment type  | api/paymentTypes/{id}                                                                                | DELETE |                    |                    |

### User Payment Options

| Description                       | Endpoint                                                                                                                                    | Method | Request Body           | Response Body          |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------- | ------ | ---------------------- | ---------------------- |
| Get a customer's payment options  | [api/userPaymentTypes?customerId={customer id}](https://bangazon-prototype-api.herokuapp.com/api/userPaymentTypes?customerId=1575559407787) | GET    |                        | UserPaymentType Array  |
| Add a payment option for customer | api/userPaymentTypes                                                                                                                        | POST   | UserPaymentType Object | UserPaymentType Object |
| Update customer payment option    | api/userPaymentTypes/{id}                                                                                                                   | PUT    | UserPaymentType Object | UserPaymentType Object |
| Remove customer payment option    | api/userPaymentTypes/{id}                                                                                                                   | DELETE |                        |                        |

### Orders \*

| Description                    | Endpoint                                                                                                                                   | Method | Request Body           | Response Body                 |
| ------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------ | ------ | ---------------------- | ----------------------------- |
| Get orders made by customer    | [api/orders?customerId={customer Id}](https://bangazon-prototype-api.herokuapp.com/api/orders?customerId=1575559407787)                    | GET    |                        | Order Array                   |
| Get order by order ID          | [api/orders/{id}](https://bangazon-prototype-api.herokuapp.com/api/orders/1575559407665)                                                   | GET    |                        | Order Object                  |
| Get customer's shopping cart   | [api/orders?customerId={customerId}&cart=true](https://bangazon-prototype-api.herokuapp.com/api/orders?customerId=1575559407787&cart=true) | GET    |                        | Order Object w/ Product Array |
| Add a product to shopping cart | api/orders                                                                                                                                 | POST   | CustomerProduct Object | Order Object                  |
| Purchase order in cart\*\*     | api/orders/{id}                                                                                                                            | PUT    | Order Object\*\*       |                               |
| Remove product from cart       | api/orders/{orderId}/products{productId}                                                                                                   | DELETE |                        |                               |

\* Order objects that have a payment method that isn't NULL are considered complete and processed. An order that does not have a payment type would be considered a user's shopping cart. A user can have only one shopping cart, and therefore will only have a maximum of one Order record in the database with a NULL payment type at a given time.

\*\* To purchase an order, update the Order object's `userPaymentId` property

### Product Types

| Description                    | Endpoint                                                                                                                              | Method | Request Body       | Response Body      |
| ------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------- | ------ | ------------------ | ------------------ |
| Get all product types          | [api/productTypes](https://bangazon-prototype-api.herokuapp.com/api/productTypes)                                                     | GET    |                    | ProductType Array  |
| Get product type by Id         | [api/productTypes/{id}](https://bangazon-prototype-api.herokuapp.com/api/productTypes/157550197010)                                   | GET    |                    | ProductType Object |
| Get product type with products | [api/productTypes/{id}?include=products](https://bangazon-prototype-api.herokuapp.com/api/productTypes/157550197010?include=products) | GET    |                    | ProductType Array  |
| Add a product type             | api/productTypes                                                                                                                      | POST   | ProductType Object | ProductType Object |
| Update a product type          | api/productTypes/{id}                                                                                                                 | PUT    | ProductType Object | ProductType Object |

### Employees

| Description                 | Endpoint                                                                                                                               | Method | Request Body    | Response Body   |
| --------------------------- | -------------------------------------------------------------------------------------------------------------------------------------- | ------ | --------------- | --------------- |
| Get all employees           | [api/employees](https://bangazon-prototype-api.herokuapp.com/api/employees)                                                            | GET    |                 | Employee Array  |
| Get employee by Id          | [api/employees/{id}](https://bangazon-prototype-api.herokuapp.com/api/employees/1575501974624)                                         | GET    |                 | Employee Object |
| Search for employee by name | [api/employees?firstName=John&lastName=Smith](http://bangazon-prototype-api.herokuapp.com/api/employees?firstName=Madi&lastName=Peper) | GET    |                 | Employee Array  |
| Add an employee             | api/employees                                                                                                                          | POST   | Employee Object | Employee Object |
| Update an employee          | api/employees/{id}                                                                                                                     | PUT    | Employee Object | Employee Object |

### Departments

| Description                   | Endpoint                                                                                                                              | Method | Request Body      | Response Body                 |
| ----------------------------- | ------------------------------------------------------------------------------------------------------------------------------------- | ------ | ----------------- | ----------------------------- |
| Get all departments           | [api/departments](http://bangazon-prototype-api.herokuapp.com/api/departments)                                                        | GET    |                   | Department Array              |
| Get department by Id          | [api/departments/{id}](http://bangazon-prototype-api.herokuapp.com/api/departments/1575559403193)                                     | GET    |                   | Department Object             |
| Get department with employees | [api/departments/{id}?include=employees](http://bangazon-prototype-api.herokuapp.com/api/departments/1575559403193?include=employees) | GET    |                   | Department Array w/ Employees |
| Add a department              | api/departments                                                                                                                       | POST   | Department Object | Department Object             |
| Update a department           | api/departments/{id}                                                                                                                  | PUT    | Department Object | Department Object             |

### Computers

| Description               | Endpoint                                                                                                   | Method | Request Body    | Response Body   |
| ------------------------- | ---------------------------------------------------------------------------------------------------------- | ------ | --------------- | --------------- |
| Get available computers   | [api/computers?available=true](http://bangazon-prototype-api.herokuapp.com/api/computers?available=true)   | GET    |                 | Computer Array  |
| Get unavailable computers | [api/computers?available=false](http://bangazon-prototype-api.herokuapp.com/api/computers?available=false) | GET    |                 | Computer Array  |
| Get computer by Id        | [api/computers/{id}](http://bangazon-prototype-api.herokuapp.com/api/computers/1575566566339)              | GET    |                 | Computer Object |
| Add computer              | api/computers                                                                                              | POST   | Computer Object | Computer Object |
| Update computer record    | api/computers/{id}                                                                                         | PUT    | Computer Object | Computer Object |
| Delete a computer record  | api/computers/{id}                                                                                         | DELETE |                 |                 |

### Training Programs

| Description                      | Endpoint                                                                                                    | Method | Request Body           | Response Body                       |
| -------------------------------- | ----------------------------------------------------------------------------------------------------------- | ------ | ---------------------- | ----------------------------------- |
| Get upcoming training programs   | [api/trainingPrograms](http://bangazon-prototype-api.herokuapp.com/api/trainingPrograms)                    | GET    |                        | TrainingProgram Array               |
| Get training program by Id       | [api/trainingPrograms/{id}](http://bangazon-prototype-api.herokuapp.com/api/trainingPrograms/1575598914183) | GET    |                        | TrainingProgram Object w/ Employees |
| Add training program             | api/trainingPrograms                                                                                        | POST   | TrainingProgram Object | Training Program Object             |
| Add employee to training program | api/trainingPrograms/{id}/employees                                                                         | POST   | Employee Object        | TrainingProgram Object w/ Employees |
| Update training program          | api/trainingPrograms/{id}                                                                                   | PUT    | TrainingProgram Object | TrainingProgram Object              |
| Remove training program          | api/trainingPrograms/{id}                                                                                   | DELETE |                        |                                     |
| Remove employee from program     | api/trainingPrograms/{id}/employees/{employeeId}                                                            | DELETE |                        |                                     |

### Revenue Report

| Description                 | Endpoint                                                                           | Method | Request Body | Response Body       |
| --------------------------- | ---------------------------------------------------------------------------------- | ------ | ------------ | ------------------- |
| Get revenue by product type | [api/revenueReport](http://bangazon-prototype-api.herokuapp.com/api/revenueReport) | GET    |              | RevenueReport Array |



## Plan

First, you need to plan. The official SQL script for creating the database is already in this repository (see below). Before you start coding, take a moment and look at the SQL script and build a Bangazon ERD using [dbdiagram.io](dbdiagram.io). Once your team agrees that the ERD is complete, you must get it approved by your lead developer before you begin writing code for the API.

## Modeling

Next, you need to author the Models needed for your API. Make sure that each model has the approprate foreign key relationship defined on it, either with a custom type or an `List<T>` to store many related things. For example, a `ProductType` has many `Products`s associated with it, and therefore you will want a property of `List<Product>` on the `ProductType` model.

## Database Management

You will be using the [Official Bangazon SQL](./Bangazon.sql) file to create your database. Create the database using Visual Studio.

## Controllers

Now it's time to build the controllers that handle GET, POST, PUT, and DELETE operations on each resource. Make sure you read, and understand, the requirements in the issue tickets.

## JSON Examples

### Customer

```js
{
    "id": 1575559407787,
    "active": true,
    "createdDate": "2019-08-25T00:00:00.000Z",
    "firstName": "Nathanael",
    "lastName": "Laverenz",
    "address": "401 Nunya Business Dr",
    "city": "Herman",
    "state": "New York",
    "email": "n.lav@sbcglobal.net",
    "phone": "6151237584"
}
```

### Customer w/ Products

```js
{
    "id": 1575559407733,
    "active": true,
    "createdDate": "2019-08-25T00:00:00.000Z",
    "firstName": "Kimble",
    "lastName": "Peskett",
    "address": "508 Loop Cir",
    "city": "Nashville",
    "state": "Tennessee",
    "email": "peskykimble@hotmail.com",
    "phone": "5671234567",
    "products": [
        {
            "id": 15755594079866,
            "productTypeId": 1575501970047,
            "customerId": 1575559407733,
            "price": 76.91,
            "description": "morbi ut odio cras mi pede malesuada in imperdiet et commodo",
            "title": "Passat",
            "dateAdded": "2019-08-25T00:00:00.000Z"
        },
        {
            "productTypeId": 1575559407749,
            "customerId": 1575559407733,
            "price": 79.92,
            "description": "semper rutrum nulla nunc purus phasellus in felis donec semper sapien a libero",
            "title": "Santa Fe",
            "dateAdded": "2019-09-25T00:00:00.000Z",
            "id": 1575669305071
        }
    ]
}
```

### Product

```js
{
    "id": 15755594079867,
    "productTypeId": 1575501970045,
    "customerId": 1575559407755,
    "price": 62.54,
    "description": "pede ullamcorper augue a suscipit nulla elit ac nulla sed",
    "title": "Murciélago LP640",
    "dateAdded": "2018-12-25T00:00:00.000Z"
}
```

### PaymentType

```js
{
    "id": 1575501974871,
    "name": "Mastercard",
    "active": true
}
```

### UserPaymentType

```js
{
    "id": 1575501978463,
    "customerId": 1575559407787,
    "paymentTypeId": 1575501974871,
    "acctNumber": "2234 56789 0123",
    "active": true
}
```

### Order

```js
{
    "id": 1575559407665,
    "customerId": 1575559407787,
    "userPaymentId": null
}
```

### CustomerProduct

```js
{
    "customerId": 1575559407787,
    "productId": 1575501970208
}
```

### ProductType

```js
{
    "id": 1575501970045,
    "name": "Accessories"
}
```

### Employee

```js
{
    "id": 1575501974624,
    "firstName": "Adam",
    "lastName": "Sheaffer",
    "departmentId": 1575559403192,
    "isSupervisor": false,
    "computerId": 1575566566333,
    "email": "iamtheboss@bangazon.com",
    "computer": {
        "id": 1575566566333,
        "purchaseDate": "2016-01-01T23:28:56.782Z",
        "decomissionDate": null,
        "make": "Apple",
        "model": "Macbook Pro"
    }
}
```

### Department

```js
{
    "id": 1575559403194,
    "name": "Accounting",
    "budget": 1230000
}
```

### Computers

```js
{
    "id": 1575566566333,
    "purchaseDate": "2016-01-01T23:28:56.782Z",
    "decomissionDate": null,
    "make": "Apple",
    "model": "Macbook Pro"
}
```

### TrainingProgram

```js
{
    "id": 1575559403194,
    "name": "GIS Application",
    "startDate": "2018-09-25T00:00:00.000Z",
    "endDate": "2018-10-05T00:00:00.000Z",
    "maxAttendees": 45
}
```

### RevenueReport

```js
{
    "productTypeId": 1575501970829,
    "productType": "Filters",
    "totalRevenue": 158.64
}
```

