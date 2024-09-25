# Fitness Workout API

## Overview

The Fitness Workout API is a RESTful web service built with ASP.NET Core that allows users to manage workouts and movements. This API provides functionality to:

- **Bulk Insert Workouts**: Insert a specified number of workouts into the database for testing or populating sample data.
- **Get Filtered Workouts**: Retrieve workouts based on various filter criteria such as duration, difficulty, and body region.
- **Get Workout Details**: Fetch detailed information about a specific workout, including associated movements and targeted regions.

## Prerequisites

- .NET 8.0 or later
- MySQL database
- Postman or any API testing tool

## Setup Instructions

1. **Clone the repository:**

   ```bash
   git clone https://github.com/sinancansevinc/FitnessWorkoutKompanion.git
   cd FitnessWorkoutKompanion
   ```

2. **Restore dependencies:**

   ```bash
   dotnet restore
   ```

3. **Create the database:**

   Run the SQL scripts provided below to create the necessary tables and stored procedures.

4. **Update the connection string:**

   Update the connection string in `appsettings.json` to point to your database.

5. **Run the application:**

   ```bash
   dotnet run
   ```

## API Documentation

### Authentication

- **Basic Authentication**: Use the `Authorization` header with the format `Basic <base64-encoded-credentials>`.

### Endpoints

- **POST /api/workouts/bulk-insert**: Inserts a specified number of workouts into the database.
- **GET /api/workouts/{id}**: Retrieves a workout by its ID, including movements and targeted regions.
- **GET /api/workouts**: Retrieves a list of workouts based on filter criteria.

## SQL Scripts

### Table Creation Scripts

You can find the SQL scripts for creating the necessary tables in the `sql-scripts/create-tables.sql` file. To execute the scripts, run the following command in your SQL client:

### Stored Procedure Scripts

The stored procedure scripts are located in the `sql-scripts/stored-procedures.sql` file. Execute the following command to create the stored procedures:

