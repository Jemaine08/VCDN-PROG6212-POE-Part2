**Project Name: ClaimsFlow**
Description

This project is a ClaimsFlow developed using ASP.NET Core MVC, allowing coordinators, managers, and lecturers to manage and process claims. The application supports functionalities like claim submission, verification, rejection, and status tracking. It is designed to be used by multiple roles within an organization, such as Lecturer, Coordinator, and Manager.

**The system implements features such as:**
Claim submission by lecturers
Claim verification and rejection by coordinators
Claim status tracking
File uploads for claims
User-friendly Razor Views for a seamless front-end experience
Encryption for secure file handling

**Technologies Used**

-ASP.NET Core MVC: For building the web application, handling controllers, and views.
-Entity Framework Core: For data access and manipulation.
-In-Memory Database: For simplified testing using an in-memory repository.
-FluentAssertions: For writing more readable and maintainable unit tests.
-Moq: For mocking dependencies in unit testing.
-AES Encryption: For securing file uploads.

**Features**

Lecturer:
-Submit claims with details like hours worked, hourly rate, and notes.
-Upload documents related to the claim (e.g., invoices, receipts).

Coordinator:
-View all claims submitted by lecturers.
-Verify claims and update their status to "Verified."
-Reject claims with a reason, changing their status to "Rejected."

Manager:
-Review and approve or reject claims after verification by the coordinator.

File Handling:
-Secure file upload using AES encryption for uploaded documents.

Role-based Access Control:
-Different views and access levels based on the user role (Lecturer, Coordinator, Manager).


**Installation**

To set up this project locally, follow the steps below:

Prerequisites

.NET 6.0 SDK or higher

A code editor like Visual Studio or Visual Studio Code

SQL Server or SQLite (if you plan to use a real database instead of the in-memory database)

Steps

Clone the repository:

git clone https://github.com/your-username/ClaimsManagementSystem.git


Navigate to the project directory:

cd ClaimsManagementSystem


Restore dependencies:

dotnet restore


Run the application:

dotnet run


The application will start on https://localhost:5001 (default URL). Open this in your browser.

Testing

This project includes unit tests for the core functionality. To run the tests:

Navigate to the test project (if separate from the main project):

cd ClaimsManagementSystem.Tests


Run the tests:

dotnet test


Tests are written using Moq for mocking dependencies and FluentAssertions for verifying assertions.

**Usage**

Once the application is running, you can:

Login as a Lecturer:
Submit a claim with relevant information.
Upload supporting documents for the claim.

Login as a Coordinator:
View submitted claims.
Verify or reject claims, adding reasons if needed.

Login as a Manager:
Approve or reject claims after they have been verified by the coordinator.

**File Uploads**
When submitting a claim, lecturers can upload files (e.g., receipts, invoices).
These files are encrypted using AES before being stored.

**Contributing**

If you'd like to contribute to this project:

Fork the repository

Create a new branch (git checkout -b feature-branch)

Make your changes

Commit your changes (git commit -am 'Add new feature')

Push to the branch (git push origin feature-branch)

Open a pull request

License

This project is licensed under the MIT License - see the LICENSE
 file for details.

**References**

ASP.NET Core Documentation
Microsoft. (2021) ASP.NET Core documentation: Controllers and actions. Available at: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/actions
 (Accessed: 22 October 2025).

FluentAssertions Library
FluentAssertions. (2021) FluentAssertions library. Available at: https://fluentassertions.com/
 (Accessed: 22 October 2025).

Moq Library
Moq. (2021) Moq: A library for .NET to create mock objects. Available at: https://github.com/moq/moq4
 (Accessed: 22 October 2025).

AES Encryption Documentation
Microsoft. (2024) AES Class — System.Security.Cryptography.Aes. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes
 (Accessed: 22 October 2025).
