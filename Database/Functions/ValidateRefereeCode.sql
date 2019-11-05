CREATE OR REPLACE FUNCTION validate_referee_code(
    "TimerId" uuid,
    "Code" text
) RETURNS boolean AS $$
BEGIN
   RETURN EXISTS (SELECT 1 FROM confirmation_codes WHERE timers_id = $1 AND code = $2);
END;
$$  LANGUAGE PLPGSQL
    SECURITY DEFINER
    SET search_path = public;
