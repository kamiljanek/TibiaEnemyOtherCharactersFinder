WITH cp AS (
  SELECT 
    f.character_id AS logout,
    t.character_id AS login,
    f.logout_or_login_date AS creating_date
  FROM 
    (
      SELECT ch.character_name, c.character_id, ch.logout_or_login_date
      FROM character_actions ch
      INNER JOIN characters c ON ch.character_name = c.name
      WHERE ch.is_online = false
    ) AS f
    CROSS JOIN 
    (
      SELECT character_name, c.character_id
      FROM character_actions ch
      INNER JOIN characters c ON ch.character_name = c.name
      WHERE is_online = true
    ) AS t
)

UPDATE character_correlations cc
SET number_of_matches = number_of_matches + 1, last_match_date = creating_date
FROM cp
WHERE 
  (cc.logout_character_id = cp.logout AND cc.login_character_id = cp.login)
  OR 
  (cc.logout_character_id = cp.login AND cc.login_character_id = cp.logout);