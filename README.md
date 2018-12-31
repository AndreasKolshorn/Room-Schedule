#  Student Scheduling System #

This repository contains some pre-release, C# code behind files from a scheduling project on which I was lead developer. A multi-year project, it integrates with the academic management system, CAMS Enterprise from Unit 4. The scheduling UI is a custom web interface built on ASP.NET, AJAX components and SQL Server.

With a custom database I designed and built, this tool allows the university registars office to schedule several students and a supervisor in each available exam room on particular days and times. Meta-data about the room such as the modality of medical discipline to be covered is also configured by the scheduler. 

A custom data feed from CAMS gathers courses associated with the student's degree requirements and lists them in a treeview the scheduler can select from during scheduling. The list also shows with changes of color of text, the completion status of courses along with course specific tooltips. 

Ultimately this database provides data to a seperate app on the student web portal where they can review their clinic schedule. Additional data consumers include numerous SSRS reports.
