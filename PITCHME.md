# Very Unsexy

An [unglamorous] series of technical software development workshops.

#HSLIDE

# Transactional Concurrency

#VSLIDE

> A transaction consists of a single command or a group of commands that execute as a package. In concurrent systems, there are often scenarios in which competing transactions must be catered for.

This is not easy.

#VSLIDE

## Dirty Reads

```sql
/* SESSION 1 */
BEGIN TRANSACTION;
UPDATE Person
SET FirstName = 'Steven'
WHERE LastName = 'Seagal';
WAITFOR DELAY '00:00:05.000';
ROLLBACK TRANSACTION;
SELECT FirstName, LastName
FROM Person
WHERE LastName = 'Seagal';

/* SESSION 2 */
SELECT FirstName, LastName 
FROM Person WITH (NOLOCK) 
WHERE LastName = 'Seagal';
```

#VSLIDE

## Lost Updates

```sql
/* SESSION 1*/
DECLARE @SafetyStockLevel int = 0, @Uplift int = 10;
BEGIN TRAN;
SELECT @SafetyStockLevel = SafetyStockLevel FROM Product
WHERE ProductID = 1;
SET @SafetyStockLevel = @SafetyStockLevel + @Uplift;
WAITFOR DELAY '00:00:05.000';
UPDATE Product
SET SafetyStockLevel = @SafetyStockLevel
WHERE ProductID = 1;
```

```sql
/* SESSION 2*/
DECLARE @SafetyStockLevel int = 0, @Uplift int = 100;
BEGIN TRAN;
SELECT @SafetyStockLevel = SafetyStockLevel
FROM Product
WHERE ProductID = 1;
SET @SafetyStockLevel = @SafetyStockLevel + @Uplift;
UPDATE Product
SET SafetyStockLevel = @SafetyStockLevel
WHERE ProductID = 1;
SELECT SafetyStockLevel
COMMIT TRAN;
```
