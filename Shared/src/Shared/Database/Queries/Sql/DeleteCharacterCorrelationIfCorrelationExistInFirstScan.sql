WITH online_characters AS (SELECT character_id FROM "characters" c INNER JOIN character_actions ca ON c."name" = ca.character_name WHERE is_online = true)
DELETE FROM character_correlations
WHERE logout_character_id IN
      (SELECT character_id
       FROM online_characters)
    AND login_character_id IN
        (SELECT character_id
         FROM online_characters)