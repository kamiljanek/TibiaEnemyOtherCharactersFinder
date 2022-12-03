WITH cp AS (SELECT 
 f.character_id AS logout
,t.character_id AS login
FROM 
	(SELECT ch.character_name, c.character_id
	FROM character_actions ch
	INNER JOIN characters c ON ch.character_name = c.name
	WHERE ch.is_online = false) AS f, 
	(SELECT character_name, c.character_id
	FROM character_actions ch
	INNER JOIN characters c ON ch.character_name = c.name
	WHERE is_online = true) AS t )
	
UPDATE character_correlations
SET number_of_matches = number_of_matches + 1
WHERE 
(logout_character_id IN (SELECT logout FROM cp) AND login_character_id IN (SELECT login FROM cp))
OR 
(logout_character_id IN (SELECT login FROM cp) AND login_character_id IN (SELECT logout FROM cp));


UPDATE character_correlations
SET number_of_matches = number_of_matches + 1
WHERE 
(logout_character_id IN (SELECT logout FROM cp) AND login_character_id IN (SELECT login FROM cp))
OR 
(logout_character_id IN (SELECT login FROM cp) AND login_character_id IN (SELECT logout FROM cp));