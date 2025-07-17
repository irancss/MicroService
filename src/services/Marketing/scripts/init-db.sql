-- Marketing Service Database Initialization Script
-- اسکریپت راه‌اندازی دیتابیس سرویس بازاریابی

-- Create extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create database if not exists (this runs before main connection)
SELECT 'CREATE DATABASE "MarketingDB"'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'MarketingDB');

-- Connect to MarketingDB
\c MarketingDB;

-- Create schemas for better organization
CREATE SCHEMA IF NOT EXISTS marketing;
CREATE SCHEMA IF NOT EXISTS hangfire;

-- Set default schema
SET search_path TO marketing, public;

-- Grant permissions
GRANT ALL PRIVILEGES ON SCHEMA marketing TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA hangfire TO postgres;

-- Insert initial data will be handled by Entity Framework migrations
-- This script just ensures the database and schemas exist

-- Create initial admin user segments (will be replaced by EF migrations)
-- This is just for reference
/*
INSERT INTO marketing."UserSegments" (
    "Id", "Name", "Description", "Type", "IsActive", 
    "Criteria", "CreatedAt", "UpdatedAt", "CreatedBy"
) VALUES (
    uuid_generate_v4(),
    'New Users',
    'Users who registered in the last 30 days',
    0, -- Demographic
    true,
    '[{"Field": "RegistrationDate", "Operator": "greater_than", "Value": "30_days_ago"}]',
    NOW(),
    NOW(),
    'system'
) ON CONFLICT DO NOTHING;
*/

-- Create indexes for better performance (EF will handle this)
-- CREATE INDEX CONCURRENTLY IF NOT EXISTS "IX_Campaigns_Slug" ON marketing."Campaigns" ("Slug");
-- CREATE INDEX CONCURRENTLY IF NOT EXISTS "IX_Campaigns_Status" ON marketing."Campaigns" ("Status");
-- CREATE INDEX CONCURRENTLY IF NOT EXISTS "IX_UserSegmentMemberships_UserId" ON marketing."UserSegmentMemberships" ("UserId");

COMMIT;
