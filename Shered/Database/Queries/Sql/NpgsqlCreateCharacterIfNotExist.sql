INSERT INTO characters (name, world_id)
SELECT DISTINCT character_name, world_id
FROM character_actions ca
WHERE NOT EXISTS (
          SELECT Name
          FROM characters c
          WHERE ca.character_name = c.name);