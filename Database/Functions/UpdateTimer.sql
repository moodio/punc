CREATE OR REPLACE FUNCTION update_timer(
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
    UPDATE timers
    SET status = $2,
    arrival_time_utc = $3,
    confirmation_method = $4,
    customer_email = $5,
    customer_name = $6,
    destination = $7,
    departure_time_utc = $8,
    estimated_arrival_time_utc = $9,
    expert_mode = $10,
    last_update_utc = $11,
    origin = $12,
    referee_email = $13,
    travel_mode = $14,
    travel_distance = $15,
    travel_duration = $16,
    payment_intent_id = $17,
    errors = $18
    WHERE id = $1
    RETURNING * INTO new_timer;
    RETURN new_timer;
end;
$$  LANGUAGE PLPGSQL
    SECURITY DEFINER
    SET search_path = public;