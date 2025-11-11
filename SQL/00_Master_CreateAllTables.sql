-- =============================================
-- Master Script: Create All Database Tables
-- Description: Executes all schema and table creation scripts in order
-- =============================================
--
-- Usage:
--   1. Ensure you have created the database first (see "Tạo database.sql")
--   2. Run this script to create all schemas and tables
--   3. Run "Insert_Roles.sql" to populate roles
--
-- Note: This script is designed to be idempotent (can be run multiple times)
-- =============================================

USE [PTJ_Database]; -- Replace with your database name
GO

PRINT '========================================';
PRINT 'Starting database schema creation...';
PRINT '========================================';
PRINT '';

-- =============================================
-- 1. Create Schema: auth (Authentication)
-- =============================================
PRINT 'Step 1/5: Creating auth schema...';
GO
:r "01_Create_Schema_Auth.sql"
GO

-- =============================================
-- 2. Create Schema: core (Core system tables)
-- =============================================
PRINT '';
PRINT 'Step 2/5: Creating core schema...';
GO
:r "02_Create_Schema_Core.sql"
GO

-- =============================================
-- 3. Create Schema: seeker (Student profiles)
-- =============================================
PRINT '';
PRINT 'Step 3/5: Creating seeker schema...';
GO
:r "03_Create_Schema_Seeker.sql"
GO

-- =============================================
-- 4. Create Schema: org (Companies)
-- =============================================
PRINT '';
PRINT 'Step 4/5: Creating org schema...';
GO
:r "04_Create_Schema_Org.sql"
GO

-- =============================================
-- 5. Create Schema: jobs (Job postings)
-- =============================================
PRINT '';
PRINT 'Step 5/5: Creating jobs schema...';
GO
:r "05_Create_Schema_Jobs.sql"
GO

PRINT '';
PRINT '========================================';
PRINT 'Database schema creation completed!';
PRINT '========================================';
PRINT '';
PRINT 'Summary of created schemas:';
PRINT '  - auth: Users, Roles, UserRoles, RefreshTokens';
PRINT '  - core: Files';
PRINT '  - seeker: Profiles, ProfileSkills, ProfileExperiences, ProfileEducations, ProfileCertificates';
PRINT '  - org: Companies, CompanyRegistrationRequests';
PRINT '  - jobs: JobPosts, JobShifts, JobPostSkills';
PRINT '';
PRINT 'Next steps:';
PRINT '  1. Run "Insert_Roles.sql" to populate initial roles';
PRINT '  2. Verify tables with: SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA IN (''auth'', ''core'', ''seeker'', ''org'', ''jobs'')';
GO
