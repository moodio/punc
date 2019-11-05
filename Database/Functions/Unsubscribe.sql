CREATE OR REPLACE FUNCTION unsubscribe(
    "Email" text
) RETURNS bool as $$
BEGIN
    DELETE FROM subscribers
    WHERE email = $1::citext;
    IF NOT EXISTS (SELECT 1 FROM do_not_disturb WHERE email = $1::citext) THEN
        INSERT INTO do_not_disturb(email)
        VALUES($1::citext);
    END IF;
    RETURN TRUE;
END;
$$  LANGUAGE plpgsql
    SECURITY DEFINER
    SET SEARCH_PATH = public;