-- Premium Subscription and Free Shipping Rules Migration
-- Run this after the build is successful

-- Create PremiumSubscriptions table
CREATE TABLE "PremiumSubscriptions" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "UserId" text NOT NULL,
    "Name" text NOT NULL,
    "SubscriptionType" integer NOT NULL,
    "Status" integer NOT NULL,
    "FreeRequestsPerMonth" integer NOT NULL,
    "UsedRequestsThisMonth" integer NOT NULL DEFAULT 0,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT now(),
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_PremiumSubscriptions" PRIMARY KEY ("Id")
);

-- Create SubscriptionUsageLogs table
CREATE TABLE "SubscriptionUsageLogs" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "SubscriptionId" uuid NOT NULL,
    "ShipmentId" uuid NOT NULL,
    "Description" text NOT NULL,
    "UsedAt" timestamp with time zone NOT NULL DEFAULT now(),
    CONSTRAINT "PK_SubscriptionUsageLogs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_SubscriptionUsageLogs_PremiumSubscriptions_SubscriptionId" FOREIGN KEY ("SubscriptionId") REFERENCES "PremiumSubscriptions" ("Id") ON DELETE CASCADE
);

-- Create FreeShippingRules table
CREATE TABLE "FreeShippingRules" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "Name" text NOT NULL,
    "Description" text NOT NULL,
    "IsActive" boolean NOT NULL DEFAULT true,
    "Priority" integer NOT NULL DEFAULT 1,
    "DiscountType" integer NOT NULL,
    "DiscountValue" decimal(18,2) NOT NULL,
    "ValidFrom" timestamp with time zone,
    "ValidTo" timestamp with time zone,
    "MaxUsageCount" integer,
    "CurrentUsageCount" integer NOT NULL DEFAULT 0,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT now(),
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_FreeShippingRules" PRIMARY KEY ("Id")
);

-- Create FreeShippingConditions table
CREATE TABLE "FreeShippingConditions" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "RuleId" uuid NOT NULL,
    "FieldName" text NOT NULL,
    "ConditionType" integer NOT NULL,
    "Operator" integer NOT NULL,
    "ValueType" integer NOT NULL,
    "Value" text NOT NULL,
    CONSTRAINT "PK_FreeShippingConditions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_FreeShippingConditions_FreeShippingRules_RuleId" FOREIGN KEY ("RuleId") REFERENCES "FreeShippingRules" ("Id") ON DELETE CASCADE
);

-- Create indexes
CREATE INDEX "IX_PremiumSubscriptions_UserId" ON "PremiumSubscriptions" ("UserId");
CREATE INDEX "IX_PremiumSubscriptions_Status" ON "PremiumSubscriptions" ("Status");
CREATE INDEX "IX_SubscriptionUsageLogs_SubscriptionId" ON "SubscriptionUsageLogs" ("SubscriptionId");
CREATE INDEX "IX_FreeShippingRules_IsActive" ON "FreeShippingRules" ("IsActive");
CREATE INDEX "IX_FreeShippingRules_Priority" ON "FreeShippingRules" ("Priority");
CREATE INDEX "IX_FreeShippingConditions_RuleId" ON "FreeShippingConditions" ("RuleId");
