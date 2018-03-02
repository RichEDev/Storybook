ALTER TABLE signoffs
ADD CONSTRAINT CK_Signoffs_ClaimPercentageToValidate_Range CHECK (
   ClaimPercentageToValidate >= 0 AND ClaimPercentageToValidate <= 100 
)