INSERT INTO characters (name, world_id)
SELECT DISTINCT ca.character_name, ca.world_id
FROM character_actions ca
LEFT JOIN characters c ON ca.character_name = c.name
WHERE c.name IS NULL;