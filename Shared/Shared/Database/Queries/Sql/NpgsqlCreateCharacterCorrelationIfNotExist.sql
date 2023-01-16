WITH cp AS (
    SELECT f.character_id AS logout, t.character_id AS login, f.logout_or_login_date AS creating_date
    FROM (
        SELECT ch.character_name, c.character_id, ch.logout_or_login_date
        FROM character_actions ch
        INNER JOIN characters c ON ch.character_name = c.name
        WHERE ch.is_online = false
    ) AS f
    CROSS JOIN (
        SELECT character_name, c.character_id
        FROM character_actions ch
        INNER JOIN characters c ON ch.character_name = c.name
        WHERE is_online = true
    ) AS t
)
INSERT INTO character_correlations (logout_character_id, login_character_id, number_of_matches, create_date, last_match_date)
SELECT logout, login, 1, creating_date, creating_date
FROM cp
LEFT JOIN character_correlations cc
ON (cc.logout_character_id = logout AND cc.login_character_id = login)
    OR (cc.logout_character_id = login AND cc.login_character_id = logout)
WHERE cc.logout_character_id IS NULL;