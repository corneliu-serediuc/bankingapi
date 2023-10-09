# Requirements

## Banking System Test
 
### System Design
 
System allows for users and user accounts.
 
A user can have as many accounts as they want.
 
A user can create and delete accounts.
 
A user can deposit and withdraw from accounts.
 
An account cannot have less than $100 at any time in an account.
 
A user cannot withdraw more than 90% of their total balance from an
account in a single transaction.
 
A user cannot deposit more than 10000 dollars in a single transaction.

### Notes
 
There is no need to develop a GUI, please provide a Web API instead.  We will focus
our review on coding style, organization, testability and test coverage.
 
Do nott worry about a real database. Feel free to fake it with in-memory data
structures.
 
The completed work can be returned to us through a public repository such as
GitHub. Please setup the project so that we can run the application locally in a
container via Docker Compose.

# Running the application locally

After running the command below in the root diretory:

```
> docker-compose up
```

The application can run in a browser using the link:

```
http://localhost:5001/swagger
```

# Disclaimer

In order to complete the solution in the alloted time, there were a few compromises made:

- Authentication and authorization were not at all considered 
- Entity objects were used as output for the response models (not a good practice)
- Http protocol was used for this docker compose setup (in general, https should be used at all times)