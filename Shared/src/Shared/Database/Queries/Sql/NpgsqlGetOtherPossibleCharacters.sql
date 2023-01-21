WITH character_id_CTE AS (SELECT character_id FROM characters WHERE name = @CharacterName)

SELECT DISTINCT c.name AS other_character_name, number_of_matches, create_date, last_match_date FROM character_correlations cc 
JOIN characters c ON c.character_id = cc.login_character_id OR c.character_id = cc.logout_character_id 
WHERE (logout_character_id = (SELECT character_id FROM character_id_CTE) OR login_character_id = (SELECT character_id FROM character_id_CTE)) 
AND c.character_id <> (SELECT character_id FROM character_id_CTE)
ORDER BY number_of_matches DESC LIMIT 10