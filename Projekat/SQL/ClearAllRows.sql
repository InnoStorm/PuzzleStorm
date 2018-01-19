--Execute multiple time

USE [db_storm]
GO
DELETE FROM [dbo].[Games]
DELETE FROM [db_storm].[dbo].[Games];
DELETE FROM [db_storm].[dbo].[Pieces];
DELETE FROM [db_storm].[dbo].[Players];
DELETE FROM [db_storm].[dbo].[Puzzles];
DELETE FROM [db_storm].[dbo].[RoomProperties];
DELETE FROM [db_storm].[dbo].[Rooms];
DELETE FROM [db_storm].[dbo].[Users];
GO

