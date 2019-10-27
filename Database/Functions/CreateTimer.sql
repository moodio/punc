CREATE OR REPLACE FUNCTION create_timer(
    "Id" uuid,
    "Status" int,
    "ArrivalTimeUtc" timestamp without time zone,
    "ConfirmationMethod" int,
    "CustomerEmail" text,
    "CustomerName" text,
    "Destination" text,
    "DepartureTimeUtc" timestamp without time zone,
    "EstimatedArrivalTimeUtc" timestamp without time zone,
    "ExpertMode" boolean,
    "LastUpdateUtc" timestamp without time zone,
    "Origin" text,
    "RefereeEmail" text,
    "TravelMode" int,
    "TravelDistance" int,
    "TravelDuration" int,
    "PaymentIntentId" text,
    "Errors" int
) RETURNS Timers AS $$
DECLARE
    new_timer timers;
begin
    INSERT INTO timers( id,
    status,
    arrival_time_utc ,
    confirmation_method,
    customer_email,
    customer_name,
    destination,
    departure_time_utc,
    estimated_arrival_time_utc,
    expert_mode,
    last_update_utc,
    origin,
    referee_email,
    travel_mode,
    travel_distance,
    travel_duration,
    payment_intent_id,
    errors)
    VALUES($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, $15, $16, $17, $18)
    RETURNING * INTO new_timer;
    RETURN new_timer;
end;
$$  LANGUAGE PLPGSQL
    SECURITY DEFINER
    SET search_path = public;

SELECT * FROM timers;