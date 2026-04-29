# ChatAI Project

## Overview

ChatAI is a C# console-based application that integrates the OpenAI API to provide real-time conversational responses. The system maintains conversation history, applies sentiment analysis to user input, and supports persistent storage through local file saving and loading. The application is designed using a layered architecture that separates the user interface, business logic, and external services.

## Architecture

The project is organized into models, services, a console-based user interface, and a test suite. Models define the core data structures used throughout the application. Services contain the business logic including conversation management, sentiment analysis, and API communication. The UI layer provides the interactive console experience for the user. The test project validates core functionality such as message handling and persistence.

## Features

The application supports real-time chat interaction with the OpenAI API while maintaining full conversation history throughout the session. Each user message is analyzed for sentiment and stored alongside its classification. Conversations can be saved to and loaded from a local JSON file, allowing persistence between sessions. The system also includes message search functionality and conversation statistics based on sentiment distribution.

## Usage

Once the application is started, users interact through a console interface by entering messages or using built-in commands. Commands allow users to control the application, including viewing help, exiting the program, clearing conversation history, saving and loading chats, viewing statistics, and searching messages. Any input that is not a command is treated as a message sent to the AI service.

## Testing

Unit tests are included to verify core functionality such as message storage, sentiment assignment, and persistence operations. Tests can be executed using the .NET test runner.

## Project Structure

The Models folder contains the data structures such as Message and Conversation. The Services folder contains the core logic for API communication, conversation handling, and sentiment analysis. The UI folder contains the console interface responsible for user interaction. The Tests folder contains unit tests for system validation. Program.cs serves as the application entry point and configures dependency injection and service initialization.

## Requirements

The project requires .NET 6 or later and an OpenAI API key configured through environment variables or application settings. An active internet connection is required for API communication.

## Purpose

This project demonstrates the application of layered architecture, dependency injection, asynchronous programming, API integration, and unit testing within a C# console application.
