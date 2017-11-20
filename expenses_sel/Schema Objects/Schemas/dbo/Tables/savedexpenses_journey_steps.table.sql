CREATE TABLE [dbo].[savedexpenses_journey_steps] (
    [expenseid]                            INT             NOT NULL,
    [step_number]                          TINYINT         NOT NULL,
    [start_location]                       INT             NULL,
    [end_location]                         INT             NULL,
    [num_miles]                            DECIMAL (18, 2) NOT NULL,
    [num_passengers]                       TINYINT         NULL,
    [exceeded_recommended_mileage]         BIT             NOT NULL,
    [exceeded_recommended_mileage_comment] NVARCHAR (4000) NULL,
    [numActualMiles]                       DECIMAL (18, 2) NOT NULL,
    [heavyBulkyEquipment]                  BIT             NOT NULL
);

