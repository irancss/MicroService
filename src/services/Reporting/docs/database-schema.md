# Reporting Service Data Warehouse Schema

## Overview
This document describes the star schema implementation for the Reporting microservice analytical database. The schema is optimized for OLAP queries and supports complex analytical queries for business intelligence.

## Star Schema Design

### Fact Tables

#### 1. order_facts
**Purpose**: Central fact table containing order transaction data for analytics

| Column | Type | Description |
|--------|------|-------------|
| id | UUID | Primary key |
| order_id | UUID | Original order identifier from source system |
| customer_id | UUID | Customer identifier from source system |
| order_date | TIMESTAMP | When the order was placed |
| total_amount | DECIMAL(18,2) | Total order amount |
| currency | VARCHAR(3) | Currency code (ISO 4217) |
| status | VARCHAR(50) | Order status |
| total_items | INTEGER | Number of items in order |
| product_dimension_id | UUID | Foreign key to product_dimensions |
| customer_dimension_id | UUID | Foreign key to customer_dimensions |
| date_dimension_id | UUID | Foreign key to date_dimensions |
| revenue | DECIMAL(18,2) | Revenue amount (measure) |
| tax | DECIMAL(18,2) | Tax amount (measure) |
| discount | DECIMAL(18,2) | Discount amount (measure) |
| created_at | TIMESTAMP | Record creation timestamp |
| updated_at | TIMESTAMP | Record update timestamp |

**Indexes**:
- `idx_order_facts_order_id` on (order_id)
- `idx_order_facts_customer_id` on (customer_id)
- `idx_order_facts_order_date` on (order_date)
- `idx_order_facts_date_currency` on (order_date, currency)
- `idx_order_facts_dimensions` on (product_dimension_id, customer_dimension_id, date_dimension_id)
- `idx_order_facts_analytics` on (date_dimension_id, currency, status)

#### 2. daily_sales_aggregates
**Purpose**: Pre-aggregated daily sales data for fast reporting queries

| Column | Type | Description |
|--------|------|-------------|
| id | UUID | Primary key |
| date | DATE | Aggregation date |
| total_revenue | DECIMAL(18,2) | Total revenue for the day |
| total_tax | DECIMAL(18,2) | Total tax for the day |
| total_discount | DECIMAL(18,2) | Total discount for the day |
| total_orders | INTEGER | Number of orders for the day |
| total_items | INTEGER | Total items sold for the day |
| average_order_value | DECIMAL(18,2) | Average order value for the day |
| currency | VARCHAR(3) | Currency code |
| created_at | TIMESTAMP | Record creation timestamp |
| updated_at | TIMESTAMP | Record update timestamp |

**Indexes**:
- `idx_daily_sales_date_currency` UNIQUE on (date, currency)

### Dimension Tables

#### 1. date_dimensions
**Purpose**: Time dimension for temporal analytics

| Column | Type | Description |
|--------|------|-------------|
| id | UUID | Primary key |
| date | DATE | Calendar date |
| year | INTEGER | Year (e.g., 2024) |
| month | INTEGER | Month (1-12) |
| day | INTEGER | Day of month (1-31) |
| quarter | INTEGER | Quarter (1-4) |
| week_of_year | INTEGER | Week number in year |
| day_of_week | VARCHAR(20) | Day name (Monday, Tuesday, etc.) |
| month_name | VARCHAR(20) | Month name (January, February, etc.) |
| is_weekend | BOOLEAN | True if Saturday or Sunday |
| is_holiday | BOOLEAN | True if marked as holiday |
| created_at | TIMESTAMP | Record creation timestamp |
| updated_at | TIMESTAMP | Record update timestamp |

**Indexes**:
- `idx_date_dimensions_date` UNIQUE on (date)
- `idx_date_dimensions_year` on (year)
- `idx_date_dimensions_month` on (month)
- `idx_date_dimensions_quarter` on (quarter)

#### 2. customer_dimensions
**Purpose**: Customer dimension for customer analytics

| Column | Type | Description |
|--------|------|-------------|
| id | UUID | Primary key |
| customer_id | UUID | Original customer ID from source system |
| email | VARCHAR(255) | Customer email |
| first_name | VARCHAR(100) | Customer first name |
| last_name | VARCHAR(100) | Customer last name |
| country | VARCHAR(100) | Customer country |
| city | VARCHAR(100) | Customer city |
| segment | VARCHAR(50) | Customer segment (Premium, Standard, Basic) |
| registration_date | TIMESTAMP | When customer registered |
| is_active | BOOLEAN | Customer active status |
| created_at | TIMESTAMP | Record creation timestamp |
| updated_at | TIMESTAMP | Record update timestamp |

**Indexes**:
- `idx_customer_dimensions_customer_id` UNIQUE on (customer_id)
- `idx_customer_dimensions_email` on (email)
- `idx_customer_dimensions_country` on (country)
- `idx_customer_dimensions_segment` on (segment)

#### 3. product_dimensions
**Purpose**: Product dimension for product analytics

| Column | Type | Description |
|--------|------|-------------|
| id | UUID | Primary key |
| product_id | UUID | Original product ID from source system |
| name | VARCHAR(255) | Product name |
| category | VARCHAR(100) | Product category |
| sub_category | VARCHAR(100) | Product sub-category |
| brand | VARCHAR(100) | Product brand |
| price | DECIMAL(18,2) | Product price |
| currency | VARCHAR(3) | Price currency |
| is_active | BOOLEAN | Product active status |
| created_at | TIMESTAMP | Record creation timestamp |
| updated_at | TIMESTAMP | Record update timestamp |

**Indexes**:
- `idx_product_dimensions_product_id` UNIQUE on (product_id)
- `idx_product_dimensions_category` on (category)
- `idx_product_dimensions_brand` on (brand)
- `idx_product_dimensions_is_active` on (is_active)

## Query Patterns

### 1. Daily Sales Report Query
```sql
SELECT 
    dd.date,
    dsa.total_revenue,
    dsa.total_orders,
    dsa.average_order_value
FROM daily_sales_aggregates dsa
JOIN date_dimensions dd ON dsa.date = dd.date
WHERE dd.date BETWEEN @from_date AND @to_date
    AND dsa.currency = @currency
ORDER BY dd.date;
```

### 2. Top Selling Products Query
```sql
SELECT 
    pd.product_id,
    pd.name,
    pd.category,
    pd.brand,
    SUM(of.revenue) as total_revenue,
    SUM(of.total_items) as total_quantity,
    COUNT(DISTINCT of.order_id) as total_orders
FROM order_facts of
JOIN product_dimensions pd ON of.product_dimension_id = pd.id
JOIN date_dimensions dd ON of.date_dimension_id = dd.id
WHERE dd.date BETWEEN @from_date AND @to_date
    AND of.currency = @currency
    AND (@category IS NULL OR pd.category = @category)
    AND (@brand IS NULL OR pd.brand = @brand)
GROUP BY pd.product_id, pd.name, pd.category, pd.brand
ORDER BY total_revenue DESC
LIMIT @top_count;
```

### 3. Customer Segment Analysis Query
```sql
SELECT 
    cd.segment,
    cd.country,
    COUNT(DISTINCT of.order_id) as total_orders,
    SUM(of.revenue) as total_revenue,
    AVG(of.revenue) as average_order_value
FROM order_facts of
JOIN customer_dimensions cd ON of.customer_dimension_id = cd.id
JOIN date_dimensions dd ON of.date_dimension_id = dd.id
WHERE dd.date BETWEEN @from_date AND @to_date
    AND of.currency = @currency
GROUP BY cd.segment, cd.country
ORDER BY total_revenue DESC;
```

## Performance Considerations

### 1. Partitioning Strategy
For large datasets, consider partitioning the `order_facts` table by date:
```sql
-- Example PostgreSQL partitioning
CREATE TABLE order_facts_2024_01 PARTITION OF order_facts
FOR VALUES FROM ('2024-01-01') TO ('2024-02-01');
```

### 2. Materialized Views
Create materialized views for frequently accessed aggregations:
```sql
CREATE MATERIALIZED VIEW monthly_sales_summary AS
SELECT 
    dd.year,
    dd.month,
    of.currency,
    SUM(of.revenue) as total_revenue,
    COUNT(DISTINCT of.order_id) as total_orders
FROM order_facts of
JOIN date_dimensions dd ON of.date_dimension_id = dd.id
GROUP BY dd.year, dd.month, of.currency;
```

### 3. Column Store Indexes (PostgreSQL)
For analytical queries, consider using column-oriented indexes:
```sql
CREATE INDEX idx_order_facts_analytics_columnar 
ON order_facts USING btree (date_dimension_id, currency, revenue);
```

## Data Retention Policy

- **Raw Facts**: Retain for 3 years (configurable)
- **Daily Aggregates**: Retain for 5 years
- **Monthly Aggregates**: Retain indefinitely
- **Dimension Data**: Retain with SCD Type 2 for historical accuracy

## ETL Process Flow

1. **Extract**: Receive OrderCompletedEvent via MassTransit
2. **Transform**: 
   - Validate and clean data
   - Lookup/create dimension records
   - Calculate measures (revenue, tax, discount)
3. **Load**: 
   - Insert fact records
   - Update dimension records if necessary
4. **Aggregate**: 
   - Run daily aggregation jobs via Hangfire
   - Update pre-calculated summaries

## Schema Evolution

- Use database migrations for schema changes
- Maintain backward compatibility for queries
- Version dimension tables for historical accuracy
- Archive old partitions instead of deleting data
