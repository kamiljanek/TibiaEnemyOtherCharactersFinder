SELECT name, url, is_available
FROM worlds
WHERE (@Available IS NULL OR is_available = @Available)
ORDER BY name