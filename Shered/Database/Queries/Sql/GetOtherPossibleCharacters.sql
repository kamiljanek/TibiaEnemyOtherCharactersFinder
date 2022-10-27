WITH cor AS (
SELECT TOP 10 CorrelationId
      ,f.Name AS LogoutName
      ,t.Name AS LoginName
      ,NumberOfMatches
  FROM CharacterCorrelations cc
  INNER JOIN Characters f on f.CharacterId = cc.LogoutCharacterId
  INNER JOIN Characters t on t.CharacterId = cc.LoginCharacterId
  where t.Name = @CharacterName OR f.Name = @CharacterName
  ORDER BY NumberOfMatches DESC
  )

  SELECT CorrelationId
        ,LoginName AS OtherCharacterName
        ,NumberOfMatches
  FROM cor
  WHERE NOT LoginName = @CharacterName
  
  UNION

  SELECT CorrelationId
        ,LogoutName AS OtherCharacterName
        ,NumberOfMatches
  FROM cor
  WHERE NOT LogoutName = @CharacterName
  ORDER BY NumberOfMatches DESC
