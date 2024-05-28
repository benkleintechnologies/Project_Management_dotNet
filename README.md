# MS Windows mini-project

## Introduction

This project is a comprehensive .NET application developed as part of a hands-on course aimed at becoming acquainted with industrial-level software architecture and design. The project covers the complete lifecycle of a .NET project, including a visualization layer built with WPF.

## Table of Contents

- [Introduction](#introduction)
- [Table of Contents](#table-of-contents)
- [Installation](#installation)
- [Usage](#usage)
- [Features](#features)
- [Dependencies](#dependencies)
- [Configuration](#configuration)
- [Examples](#examples)
- [Contributors](#contributors)

## Installation

To set up the project locally, follow these steps:

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-repository/project-management-dotnet.git
   ```
2. **Install Visual Studio 2022** with .NET 7.0 and C# 11.
3. **Open the solution file** `dotNet5784_8589_0954.sln` in Visual Studio.
4. **Restore NuGet packages** if required.
5. **Build the solution** to ensure all dependencies are resolved.

## Usage

Run the application through Visual Studio by setting the appropriate project as the startup project and running it. Follow these detailed steps for common operations:

### Running the Application

1. Open the solution in Visual Studio.
2. Set the startup project to `PL` (Presentation Layer).
3. Press `F5` to start debugging, or `Ctrl+F5` to run without debugging.

### Common Operations

#### Adding a New Task

1. Navigate to the task management section of the application.
2. Click on "Add New Task."
3. Fill in the task details including name, start date, end date, and any dependencies.
4. Click "Save" to add the task to the project.

#### Updating a Task's Dates

1. Select the task you want to update.
2. Modify the start date or end date as required.
3. The system will automatically update any dependent tasks based on the new dates.
4. Click "Update" to save changes.

#### Viewing the Gantt Chart

1. Navigate to the Gantt Chart section.
2. Tasks will be displayed with color coding based on their status.
3. Use the scroll features to explore different time frames.

## Features

### Core Features

- **Full .NET 7.0 structure with C# 11:** Leveraging the latest .NET framework and C# language features for modern application development.
- **Data Handling with LINQ and XML:** Efficient data collection management and querying using LINQ. XML DOM manipulation for data storage and retrieval.
- **GUI Development with WPF:** Building a robust and user-friendly graphical user interface using the Windows Presentation Foundation (WPF) framework.
- **Multithreaded Development:** Implementing multithreading to enhance application performance and responsiveness.
- **Design Patterns:** Applying essential design patterns such as Singleton, Factory Method, Dependency Injection, and Observer to ensure a scalable and maintainable architecture.
- **Multi-tier Architecture:** Structuring the application into separate layers for data access, business logic, and presentation to promote separation of concerns and reusability.

### Bonus Features

- **Automatic Schedule Creation with Milestones:** Automating the creation of project schedules, including milestone tracking.
- **Support for Projected End Date and Deadline:** Adding functionality to track and manage project deadlines and projected end dates.
- **InJeopardy Status:** Introducing a status indicator for tasks that are at risk of not meeting deadlines.
- **Update Chain of Tasks:** Automatically updating dependent tasks when a task's start or end date is modified.
- **Deletion Flag:** Implementing a deletion flag for soft-deleting tasks or records.
- **Lazy Singleton Implementation:** Utilizing lazy initialization for the Singleton pattern to ensure efficient resource usage.
- **Custom Styling:** Enhancing the user interface with custom styles to improve aesthetics and usability.
- **Circular Dependency Check:** Adding a feature to detect and handle circular dependencies in project tasks.
- **Color-coded Gantt Chart:** Displaying a Gantt chart with tasks color-coded by their status for better visualization.

## Dependencies

- .NET 7.0
- Visual Studio 2022
- C# 11
- Git

## Configuration

Configuration details such as app settings and connection strings can be found in the `appsettings.json` file and adjusted as necessary for your environment.

## Examples: 

### Main Window
<img width="836" alt="Main Window Screenshot" src="https://github.com/benkleintechnologies/Project_Management_dotNet/assets/13645974/daf84b01-e016-4268-be19-c9e33365987a">

### Admin View
<img width="836" alt="Admin Window Screenshot" src="https://github.com/benkleintechnologies/Project_Management_dotNet/assets/13645974/a23de356-3167-4a79-a5e9-c41260696542">

### Gantt Chart
<img width="2566" alt="Gannt Chart Unstarted Screenshot" src="https://github.com/benkleintechnologies/Project_Management_dotNet/assets/13645974/8bb2d7e2-98f9-4b6f-b376-286330ef7afb">

#### Gantt Chart once Task has started and there are delays
<img width="2599" alt="Gannt Chart Delays Screenshot" src="https://github.com/benkleintechnologies/Project_Management_dotNet/assets/13645974/9d65d7eb-c766-4d12-a6ff-7ea70f215d4e">

### Engineer Window
<img width="836" alt="Engineer List Screenshot" src="https://github.com/benkleintechnologies/Project_Management_dotNet/assets/13645974/2db214f4-ff1e-431c-a2b2-a6f22f8540b3">
<img width="435" alt="Individual Engineer Screenshot" src="https://github.com/benkleintechnologies/Project_Management_dotNet/assets/13645974/41bb7680-9011-473b-baff-58d742646849">

### Engineer View
<img width="836" alt="Engineer View Screenshot" src="https://github.com/benkleintechnologies/Project_Management_dotNet/assets/13645974/2c44392c-216a-4f3b-af4d-81984d491e43">


## Contributors

- [Eli Isaacs](https://github.com/eisaacs7)
