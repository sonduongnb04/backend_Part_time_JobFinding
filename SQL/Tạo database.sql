IF DB_ID('PartTimeJobs') IS NULL
BEGIN
    CREATE DATABASE PartTimeJobs;
END
GO
USE PartTimeJobs;
GO

-- Schemas
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'auth')  EXEC('CREATE SCHEMA auth');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'org')   EXEC('CREATE SCHEMA org');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'seeker')EXEC('CREATE SCHEMA seeker');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'jobs')  EXEC('CREATE SCHEMA jobs');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'comms') EXEC('CREATE SCHEMA comms');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'ops')   EXEC('CREATE SCHEMA ops');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'core')  EXEC('CREATE SCHEMA core');
GO

