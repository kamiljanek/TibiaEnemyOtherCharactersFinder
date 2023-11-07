SELECT c.name
FROM characters c
WHERE c.name >= @SearchText
ORDER BY c.name
OFFSET ((@Page - 1) * @PageSize) ROWS
    LIMIT @PageSize;