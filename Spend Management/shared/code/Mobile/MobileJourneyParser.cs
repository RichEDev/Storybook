namespace Spend_Management.shared.code.Mobile
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web.Script.Serialization;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Mileage;
    using SpendManagementLibrary.Mobile;

    using shared.webServices;

    /// <summary>
    /// A parser to handle mobile journey steps
    /// </summary>
    public class MobileJourneyParser
    {
        /// <summary>
        /// Loops through the JSON for this mobile expense item and creates valid journey steps for the mileage grid
        /// </summary>
        /// <param name="item">The <see cref="ExpenseItem"/> with steps to parse.</param>
        /// <param name="generalOptionsFactory">An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/></param>
        /// <returns>A List of <see cref="cJourneyStep"/></returns>
        public static List<cJourneyStep> GetAddressSuggestionsForMobileExpenseItem(ExpenseItem item, IDataFactory<IGeneralOptions, int> generalOptionsFactory)
        {
            MobileJourney journey = new JavaScriptSerializer().Deserialize<MobileJourney>(item.JourneySteps);

            var suggestions = GetAddressSuggestionsForMobileJourney(journey, generalOptionsFactory);

            return suggestions;
        }

        /// <summary>
        /// Loops through all of the JSON for the journey from the mobile device and creates valid journey steps for the mileage grid
        /// </summary>
        /// <param name="journey">
        /// The journey.
        /// </param>
        /// <param name="generalOptionsFactory">An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/></param>
        /// <returns>
        /// The <see cref="MobileJourney"/> with search results.
        /// </returns>
        public static List<cJourneyStep> GetAddressSuggestionsForMobileJourney(MobileJourney journey, IDataFactory<IGeneralOptions, int> generalOptionsFactory)
        {
            List<MobileJourneyStep> journeySteps = new JavaScriptSerializer().Deserialize<List<MobileJourneyStep>>(journey.JourneyJson);
            journey.Steps = new List<MobileJourneyStep>();

            var user = cMisc.GetCurrentUser();
            var globalCountries = new cGlobalCountries();
            var localCountries = new cCountries(user.AccountID, user.Employee.DefaultSubAccount);
            var subAccounts = new cAccountSubAccounts(user.AccountID);
            var subAccount = subAccounts.getSubAccountById(user.CurrentSubAccountId);
            var svcAddresses = new svcAddresses();
            var pca = new PostcodeAnywhere(user.AccountID);

            // add the home addresses and associated dates
            var employeeHomeLocations = (user.Delegate == null) ? new EmployeeHomeAddresses(user.AccountID, user.EmployeeID).Get() : null;
            var homeAddresses = new List<svcAddresses.HomeOrOfficeAddress>();
            var officeAddresses = new List<svcAddresses.HomeOrOfficeAddress>();
            if (employeeHomeLocations != null)
            {
                homeAddresses.AddRange(
                    from homeLocation in employeeHomeLocations
                    let homeAddress = Address.Get(user.AccountID, homeLocation.LocationID)
                    select
                        new svcAddresses.HomeOrOfficeAddress
                            {
                                Identifier = homeAddress.Identifier, 
                                GlobalIdentifier = homeAddress.GlobalIdentifier, 
                                StartDate = homeLocation.StartDate.ToString(), 
                                EndDate = homeLocation.EndDate.ToString(), 
                                AddressFriendlyName = homeAddress.FriendlyName
                            });
            }

            // add the office addresses and associated dates
            var employeeOfficeLocations = (user.Delegate == null) ? new EmployeeWorkAddresses(user.AccountID, user.EmployeeID).Get() : null;
            if (employeeOfficeLocations != null)
            {
                officeAddresses.AddRange(
                    from officeLocation in employeeOfficeLocations.Values
                    let officeAddress = Address.Get(user.AccountID, officeLocation.LocationID)
                    select
                        new svcAddresses.HomeOrOfficeAddress
                            {
                                Identifier = officeAddress.Identifier, 
                                GlobalIdentifier = officeAddress.GlobalIdentifier, 
                                StartDate = officeLocation.StartDate.ToString(), 
                                EndDate = officeLocation.EndDate.ToString(), 
                                AddressFriendlyName = officeAddress.FriendlyName
                            });
            }

            Dictionary<string, cGlobalCountry> countries = new Dictionary<string, cGlobalCountry>();

            foreach (MobileJourneyStep step in journeySteps)
            {
                cGlobalCountry country;

                if (string.IsNullOrEmpty(step.Country))
                {
                    int primaryCountryId = user.Employee.PrimaryCountry;

                    if (primaryCountryId == 0)
                    {
                        // no primary country is set for the employee, so use account country
                        var generalOptions = generalOptionsFactory[user.CurrentSubAccountId].WithCountry();

                        primaryCountryId = generalOptions.Country.HomeCountry;
                    }
                
                    country = 
                        globalCountries.getGlobalCountryById(
                            localCountries.getCountryById(primaryCountryId).GlobalCountryId);

             
                    if (country != null)
                    {
                        step.Country = country.CountryCode;

                        if (countries.ContainsKey(country.CountryCode) == false)
                        {
                            countries.Add(country.CountryCode, country);
                        }
                    }
                }
                else
                {
                    countries.TryGetValue(step.Country, out country);
                }

                if (country == null)
                {
                    country = globalCountries.getGlobalCountryByAlphaCode(step.Country) ??
                              globalCountries.GetGlobalCountryByAlpha3Code(step.Country);

                    countries.Add(step.Country, country);
                }

                if (step.Postcode == null)
                {
                    // if there's no postcode then we don't want to save the data so create a new address instance with a negative identifier but don't save it
                    step.Address = new Address
                    {
                        Identifier = -100,
                        City = step.City,
                        Postcode = step.Postcode,
                        Line1 = step.Line1,
                        Country = country.GlobalCountryId,
                        Latitude = step.Latitude.ToString(CultureInfo.InvariantCulture),
                        Longitude = step.Longitude.ToString(CultureInfo.InvariantCulture)
                    };
                }

                else
                {
                string searchTerm = step.Postcode.Trim().Replace(" ", string.Empty);

                var selResults = svcAddresses.Autocomplete(searchTerm, country.Alpha3CountryCode, journey.JourneyDate, 0);

                if (selResults.ManualAddresses.Count > 0)
                {
                    if (homeAddresses.Count > 0)
                    {
                        step.Address = GetAddress(selResults, homeAddresses, journey.JourneyDateTime, globalCountries);
                    }

                    if (step.Address == null && officeAddresses.Count > 0)
                    {
                        // the null check above ensures we don't overwrite a valid home address that was retrieved
                        step.Address = GetAddress(selResults, officeAddresses, journey.JourneyDateTime, globalCountries);
                    }

                    if (step.Address == null)
                    {
                        // Finally, if we still don't have an address then get the first match from the manual addresses table
                        step.Address = Address.Get(user.AccountID, selResults.ManualAddresses[0].Identifier);
                    }
                }


                // We only get to the below if we didn't find a home or work address above, so then we want to hit PCA
                if (step.Address == null)
                {
                    if (step.PostcodeAnywhereResults == null)
                    {
                        step.PostcodeAnywhereResults = new List<PostcodeAnywhereResult>();
                    }

                    DataTable pcaResults = pca.CapturePlusInteractiveAutoComplete(searchTerm, country.Alpha3CountryCode);

                    foreach (var p in from DataRow row in pcaResults.Rows
                                      select
                                          new PostcodeAnywhereResult
                                          {
                                              Id = Convert.ToString(row["Id"]),
                                              Match = Convert.ToString(row["Match"]),
                                              Suggestion = Convert.ToString(row["Suggestion"]).Trim(),
                                              IsRetrievable = Convert.ToBoolean(row["IsRetrievable"])
                                          })
                    {
                        step.PostcodeAnywhereResults.Add(p);
                    }

                    if (step.PostcodeAnywhereResults.Count > 0)
                    {
                        var result = step.PostcodeAnywhereResults[0];

                        if (result.IsRetrievable)
                        {
                            Address address = Address.Get(user, result.Id, globalCountries);
                            step.Address = address;
                        }

                        else
                        {
                            DataTable pcaRetrievableResults = pca.CapturePlusInteractiveFind(result.Id);

                            List<PostcodeAnywhereResult> retrievablePostcodeAnywhereResults =
                                (from DataRow row in pcaRetrievableResults.Rows
                                 select new PostcodeAnywhereResult
                                            {
                                                Id = Convert.ToString(row["Id"]),
                                                Suggestion = Convert.ToString(row["Suggestion"]).Trim(), 
                                                IsRetrievable = Convert.ToBoolean(row["IsRetrievable"])
                                            }).ToList();

                            foreach (var postcodeAnywhereResult in retrievablePostcodeAnywhereResults.Where(postcodeAnywhereResult => postcodeAnywhereResult.IsRetrievable == true))
                            {
                                step.Address = Address.Get(user, postcodeAnywhereResult.Id, globalCountries);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (subAccount.SubAccountProperties.AddLocations)
                        {
                            // If the company has enabled manual addresses then save it to the database and retrieve the full record back
                            int addressId = svcAddresses.Save(
                                0,
                                string.Empty,
                                    string.IsNullOrEmpty(step.Line1) ? string.Empty : step.Line1,
                                string.Empty,
                                string.Empty,
                                    string.IsNullOrEmpty(step.City) ? string.Empty : step.City,
                                string.Empty,
                                country.Alpha3CountryCode,
                                step.Postcode,
                                step.Latitude.ToString(CultureInfo.InvariantCulture),
                                step.Longitude.ToString(CultureInfo.InvariantCulture),
                                string.Empty,
                                false,
                                null,
                                true);

                            if (addressId > 0)
                            {
                                step.Address = Address.Get(user.AccountID, addressId);
                            }
                        }
                        else
                        {
                            // Finally, if the company don't allow manual addresses then create a new address instance with a negative identifier but don't save it
                            step.Address = new Address
                            {
                                Identifier = -100,
                                City = step.City,
                                Postcode = step.Postcode,
                                Line1 = step.Line1,
                                Country = country.GlobalCountryId,
                                Latitude = step.Latitude.ToString(CultureInfo.InvariantCulture),
                                Longitude = step.Longitude.ToString(CultureInfo.InvariantCulture)
                            };
                        }
                    }
                }
                }


                journey.Steps.Add(step);
            }

            List<cJourneyStep> steps = new List<cJourneyStep>();

            // Loop through all of the journey steps except the last one because that's the final location and nowhere was visited afterwards.
            for (int i = 0; i < journey.Steps.Count - 1; i++)
            {
                var from = journey.Steps[i].Address;
                var to = journey.Steps[i + 1].Address;

                decimal? distanceBetweenFromAndTo = null;

                if (from.Identifier > 0 && to.Identifier > 0)
                {
                    // this check is needed to handle the negative identifiers from above for when manual addresses can't be added
                    distanceBetweenFromAndTo = AddressDistance.GetRecommendedOrCustomDistance(
                        from,
                        to,
                        user.AccountID,
                        subAccount,
                        user);
                }

                List<Passenger> passengers = new List<Passenger>();

                decimal distance = distanceBetweenFromAndTo == null ? 0 : (decimal)distanceBetweenFromAndTo;

                if (!string.IsNullOrEmpty(journey.Steps[i + 1].PassengerNames))
                {
                    string[] passengersSplit = journey.Steps[i + 1].PassengerNames.Split(',');

                    passengers.AddRange(passengersSplit.Select(passenger => new Passenger { Name = passenger }));
                }

                var step = new cJourneyStep(
                    0,
                    from,
                    to,
                    distance,
                    distance,
                    Convert.ToByte(journey.Steps[i + 1].NumberPassengers),
                    Convert.ToByte(journey.Steps[i + 1].StepNumber),
                    distance,
                    journey.Steps[i + 1].HeavyBulkyEquipment)
                    { passengers = passengers.ToArray() };

                steps.Add(step);
            }

            return steps;
        }

        /// <summary>
        /// Finds a <see cref="svcAddresses.HomeOrOfficeAddress"/> in a list.
        /// </summary>
        /// <param name="autocompleteResults">
        /// The list of results
        /// </param>
        /// <param name="homeOrOfficeAddresses">
        /// The home or office addresses to search for
        /// </param>
        /// <param name="date">
        /// The date of the journey
        /// </param>
        /// <param name="globalCountries">
        /// The global Countries.
        /// </param>
        /// <returns>
        /// A <see cref="cAddress"/> of the users' home or office address.
        /// </returns>
        private static Address GetAddress(svcAddresses.AutocompleteResult autocompleteResults, List<svcAddresses.HomeOrOfficeAddress> homeOrOfficeAddresses, DateTime date, cGlobalCountries globalCountries)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            // Loop through all of the home addresses
            foreach (var manualAddress in autocompleteResults.ManualAddresses)
            {
                foreach (var address in homeOrOfficeAddresses)
                {
                    if (manualAddress.Identifier == address.Identifier)
                    {
                        if (date >= DateTime.Parse(address.StartDate) && (string.IsNullOrEmpty(address.EndDate) || DateTime.Parse(address.EndDate) <= date))
                        {
                            // If the identifier matches and it's valid for the date of the journey then return it
                             return Address.Get(user.AccountID, manualAddress.Identifier);
                        }
                    }

                    if (!string.IsNullOrEmpty(manualAddress.GlobalIdentifier) && manualAddress.GlobalIdentifier == address.GlobalIdentifier)
                    {
                        if (date >= DateTime.Parse(address.StartDate)  && (string.IsNullOrEmpty(address.EndDate) || DateTime.Parse(address.EndDate) <= date))
                        {
                            return Address.Get(user, manualAddress.GlobalIdentifier, globalCountries);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Converts <see cref="MobileJourneyStep"/> to <see cref="cJourneyStep"/>
        /// </summary>
        /// <param name="steps">The list of <see cref="MobileJourneyStep"/> to convert</param>
        /// <returns>A list of <see cref="cJourneyStep"/></returns>
        private static List<cJourneyStep> ConvertMobileJourneySteps(List<MobileJourneyStep> steps)
        {
            return null;
        }
    }
}