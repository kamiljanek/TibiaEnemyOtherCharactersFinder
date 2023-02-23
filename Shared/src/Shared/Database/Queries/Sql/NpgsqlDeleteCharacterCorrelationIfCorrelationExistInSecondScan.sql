WITH offline_characters AS (SELECT character_id FROM "characters" c INNER JOIN character_actions ca ON c."name" = ca.character_name WHERE is_online = false)
DELETE FROM character_correlations
WHERE logout_character_id IN
      (SELECT character_id
       FROM offline_characters)
    AND login_character_id IN
        (SELECT character_id
         FROM offline_characters)