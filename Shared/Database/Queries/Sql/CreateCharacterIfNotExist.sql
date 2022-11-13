INSERT INTO Characters (Name, WorldId)
SELECT DISTINCT CharacterName, WorldId
FROM CharacterLogoutOrLogins clol
WHERE NOT EXISTS (
          SELECT Name
          FROM Characters c
          WHERE clol.CharacterName = c.Name);