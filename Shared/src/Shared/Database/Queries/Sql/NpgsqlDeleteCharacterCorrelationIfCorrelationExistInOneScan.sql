WITH 	cte_on AS (SELECT c.character_id FROM character_actions ca LEFT JOIN characters c ON ca.character_name = c.name WHERE is_online),
      
        cte_off AS (SELECT c.character_id FROM character_actions ca LEFT JOIN characters c ON ca.character_name = c.name WHERE NOT is_online),
        
        cte_del AS (SELECT on1.character_id AS login, on2.character_id AS logout
                    FROM cte_on on1, cte_on on2
                    WHERE on1.character_id <> on2.character_id
                    UNION
                    SELECT off1.character_id, off2.character_id
                    FROM cte_off off1, cte_off off2
                    WHERE off1.character_id <> off2.character_id)

DELETE FROM character_correlations cc WHERE (cc.login_character_id, cc.logout_character_id) IN (SELECT login, logout FROM cte_del)