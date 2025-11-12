-- =============================================
-- Master Script: Insert All Sample Data
-- Description: Executes all sample data scripts in correct order
-- =============================================
--
-- Usage:
--   1. Ensure database and tables are created first
--   2. Run this script to populate with sample data
--   3. Use the sample accounts to test APIs
--
-- IMPORTANT: This script will NOT duplicate data (uses IF NOT EXISTS checks)
-- =============================================

USE [PTJ_Database]; -- Replace with your database name
GO

PRINT '========================================';
PRINT 'Starting sample data insertion...';
PRINT '========================================';
PRINT '';

-- =============================================
-- 1. Auth Schema (Users, Roles, UserRoles)
-- =============================================
PRINT 'Step 1/5: Inserting Auth sample data...';
PRINT '----------------------------------------';
GO
:r "SampleData_01_Auth.sql"
GO

-- =============================================
-- 2. Core Schema (Files)
-- =============================================
PRINT '';
PRINT 'Step 2/5: Inserting Core Files sample data...';
PRINT '----------------------------------------';
GO
:r "SampleData_02_Core_Files.sql"
GO

-- =============================================
-- 3. Seeker Schema (Profiles)
-- =============================================
PRINT '';
PRINT 'Step 3/5: Inserting Seeker Profiles sample data...';
PRINT '----------------------------------------';
GO
:r "SampleData_03_Seeker_Profiles.sql"
GO

-- =============================================
-- 4. Org Schema (Companies)
-- =============================================
PRINT '';
PRINT 'Step 4/5: Inserting Org Companies sample data...';
PRINT '----------------------------------------';
GO
:r "SampleData_04_Org_Companies.sql"
GO

-- =============================================
-- 5. Jobs Schema (JobPosts, JobShifts)
-- =============================================
PRINT '';
PRINT 'Step 5/5: Inserting Jobs sample data...';
PRINT '----------------------------------------';
GO
:r "SampleData_05_Jobs_JobPosts.sql"
GO

PRINT '';
PRINT '========================================';
PRINT 'ALL SAMPLE DATA INSERTED SUCCESSFULLY!';
PRINT '========================================';
PRINT '';
PRINT 'Summary:';
PRINT '----------------------------------------';
PRINT 'Users: 6 (1 Admin, 2 Employers, 3 Students)';
PRINT 'Roles: 3 (ADMIN, EMPLOYER, STUDENT)';
PRINT 'Files: 7 (Avatars, Resumes, Logos)';
PRINT 'Profiles: 3 student profiles';
PRINT 'Companies: 3 companies';
PRINT 'Job Posts: 5 job postings';
PRINT 'Job Shifts: 9 work shifts';
PRINT '';
PRINT 'Test Accounts:';
PRINT '----------------------------------------';
PRINT 'Admin:     admin@ptj.com / Password123!';
PRINT 'Employer1: employer1@company.com / Password123!';
PRINT 'Employer2: employer2@techcorp.com / Password123!';
PRINT 'Student1:  student1@gmail.com / Password123!';
PRINT 'Student2:  student2@gmail.com / Password123!';
PRINT 'Student3:  student3@gmail.com / Password123!';
PRINT '';
PRINT 'You can now test your APIs with these accounts!';
PRINT '========================================';
GO
