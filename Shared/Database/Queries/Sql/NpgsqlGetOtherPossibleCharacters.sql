WITH cor AS (
SELECT
       correlation_id AS CorrelationId
      ,f.name AS LogoutName
      ,t.name AS LoginName
      ,number_of_matches AS NumberOfMatches
  FROM character_correlations cc
  INNER JOIN characters f on f.character_id = cc.logout_character_id
  INNER JOIN characters t on t.character_id = cc.login_character_id
  where (t.name LIKE @CharacterName OR f.name LIKE @CharacterName)
  ORDER BY number_of_matches DESC LIMIT 10
  )

  SELECT CorrelationId
        ,LoginName AS OtherCharacterName
        ,NumberOfMatches
  FROM cor
  WHERE NOT LoginName LIKE @CharacterName
  
  UNION

  SELECT CorrelationId
        ,LogoutName AS OtherCharacterName
        ,NumberOfMatches
  FROM cor
  WHERE NOT LogoutName LIKE @CharacterName
  ORDER BY NumberOfMatches DESC
