
WITH CartesianProduct AS 
(
SELECT 
 f.CharacterId AS LogoutCharacterId
,t.CharacterId AS LoginCharacterId
FROM 
	(SELECT ch.CharacterName, c.CharacterId
	FROM CharacterLogoutOrLogins ch
	INNER JOIN Characters c ON ch.CharacterName = c.Name
	WHERE ch.IsOnline = 0) AS f, 
	(SELECT CharacterName, c.CharacterId
	FROM CharacterLogoutOrLogins ch
	INNER JOIN Characters c ON ch.CharacterName = c.Name
	WHERE IsOnline = 1) AS t 
)

 MERGE CharacterCorrelations AS target
 USING CartesianProduct AS source ON 
	(target.LogoutCharacterId = source.LogoutCharacterId AND target.LoginCharacterId = source.LoginCharacterId) OR 
	(target.LogoutCharacterId = source.LoginCharacterId AND target.LoginCharacterId = source.LogoutCharacterId)

 WHEN MATCHED
 THEN
     UPDATE SET
         target.NumberOfMatches += 1
  
 WHEN NOT MATCHED
 THEN
     INSERT (
	 LogoutCharacterId, 
	 LoginCharacterId, 
	 NumberOfMatches
	 )
     VALUES (
	 source.LogoutCharacterId, 
	 source.LoginCharacterId, 
	 1
	 );