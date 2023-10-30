WITH online_characters AS
         (SELECT character_id FROM characters c WHERE found_in_scan = true)

DELETE FROM character_correlations
WHERE logout_character_id IN
      (SELECT character_id
       FROM online_characters)

  AND login_character_id IN
      (SELECT character_id
       FROM online_characters)