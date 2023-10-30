WITH cte AS (SELECT * FROM character_correlations cc WHERE cc.logout_character_id = @p0 OR cc.login_character_id = @p0)
SELECT DISTINCT ON (cc1.correlation_id)
    json_build_object(
            'FirstCombinedCorrelation', json_build_object(
            'CorrelationId', cc1.correlation_id,
            'LogoutCharacterId', cc1.logout_character_id,
            'LoginCharacterId', cc1.login_character_id,
            'NumberOfMatches', cc1.number_of_matches,
            'CreateDate', cc1.create_date,
            'LastMatchDate', cc1.last_match_date
        ),
            'SecondCombinedCorrelation', json_build_object(
                    'CorrelationId', cc2.correlation_id,
                    'LogoutCharacterId', cc2.logout_character_id,
                    'LoginCharacterId', cc2.login_character_id,
                    'NumberOfMatches', cc2.number_of_matches,
                    'CreateDate', cc2.create_date,
                    'LastMatchDate', cc2.last_match_date
                )
        ) AS combined_json
FROM (SELECT * FROM cte) AS cc1
         INNER JOIN (SELECT * FROM cte) AS cc2 ON cc1.logout_character_id = cc2.login_character_id
    AND cc1.login_character_id = cc2.logout_character_id
    AND cc1.correlation_id <> cc2.correlation_id
WHERE cc1.logout_character_id < cc1.login_character_id;