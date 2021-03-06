﻿// Licensed under the GPL License, Version 3.0. See LICENSE in the git repository root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ecwid.Tools
{
    /// <summary>
    /// Some validators for classes.
    /// </summary>
    internal abstract class Validators
    {
        /// <summary>
        /// The available legacy fulfillment statuses.
        /// </summary>
        public static readonly IList<string> AvailableLegacyFulfillmentStatuses = new List<string>
        {
            "AWAITING_PROCESSING",
            "NEW",
            "PROCESSING",
            "SHIPPED",
            "DELIVERED",
            "WILL_NOT_DELIVER",
            "RETURNED"
        };

        /// <summary>
        /// The available fulfillment statuses.
        /// </summary>
        public static readonly IList<string> AvailableFulfillmentStatuses = new List<string>
        {
            "AWAITING_PROCESSING",
            "PROCESSING",
            "SHIPPED",
            "DELIVERED",
            "WILL_NOT_DELIVER",
            "RETURNED"
        };

        /// <summary>
        /// The available legacy payment statuses.
        /// </summary>
        public static readonly IList<string> AvailableLegacyPaymentStatuses = new List<string>
        {
            "PAID",
            "ACCEPTED",
            "DECLINED",
            "CANCELLED",
            "AWAITING_PAYMENT",
            "QUEUED",
            "CHARGEABLE, REFUNDED",
            "INCOMPLETE"
        };

        /// <summary>
        /// The available payment statuses.
        /// </summary>
        public static readonly IList<string> AvailablePaymentStatuses = new List<string>
        {
            "PAID",
            "CANCELLED",
            "AWAITING_PAYMENT",
            "REFUNDED",
            "INCOMPLETE"
        };

        /// <summary>
        /// Check strings for <see langword="null" /> or <see langword="empty" />.
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <returns>True if the all of the strings are <see langword="null" /> or <see langword="empty" /></returns>
        public static bool AreNullOrEmpty(params string[] strings)
        {
            return strings.All(IsNullOrEmpty);
        }

        /// <summary>
        /// Determines whether [is null or empty] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        public static bool IsNullOrEmpty(string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Statuseses validate.
        /// </summary>
        /// <param name="statuses">The statuses.</param>
        /// <param name="statusesAvailable">The statuses available.</param>
        /// <exception cref="ArgumentException">Statuses string is invalid.</exception>
        public static void StatusesValidate(string statuses, ICollection<string> statusesAvailable)
        {
            if (IsNullOrEmpty(statuses))
                throw new ArgumentException("Statuses string is invalid.", nameof(statuses));

            if (statusesAvailable == null)
                throw new ArgumentException("Statuses collection is invalid.", nameof(statusesAvailable));

            if (statusesAvailable.Count == 0)
                throw new ArgumentException("Statuses collection is invalid.", nameof(statusesAvailable));

            try
            {
                if (!CheckContainsString(statuses, statusesAvailable))
                    throw new ArgumentException("Statuses string is invalid.", nameof(statuses));
            }
            catch (ArgumentException argumentException)
            {
                throw new ArgumentException("Statuses string is invalid.", argumentException);
            }
        }

        /// <summary>
        /// Validate the status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="statusesAvailable">The statuses available.</param>
        /// <exception cref="ArgumentException">Status string is invalid.</exception>
        /// <exception cref="ArgumentException">Status string is invalid. Support only one status. </exception>
        public static string StatusValidate(string status, ICollection<string> statusesAvailable)
        {
            StatusesValidate(status, statusesAvailable);

            var result = status.TrimUpperReplaceSplit();

            if (result.Count > 1)
                throw new ArgumentException("Status string is invalid. Support only one status.", nameof(status));

            return result.First();
        }

        /// <summary>
        /// Checks the contains words of the string in the list.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="list">The list.</param>
        /// <exception cref="ArgumentException">Unable replace and split string</exception>
        private static bool CheckContainsString(string str, ICollection<string> list)
        {
            var result = str.TrimUpperReplaceSplit();

            return result.Aggregate(true, (current, s) => current && list.Contains(s));
        }

        /// <summary>
        /// Validates and add new parameters.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="strings">The strings.</param>
        /// <exception cref="ArgumentException">Query is empty. Prevent change all orders.</exception>
        /// <exception cref="ArgumentException">All new statuses are null or empty.</exception>
        public static void ValidateNewLegacyStatuses(
            Dictionary<string, object> query, params string[] strings)
        {
            //check query builder query params count
            var exceptionList = new List<string> {"limit", "offset"};
            var count = query.Keys.Count(s => !exceptionList.Contains(s));
            if (count == 0)
                throw new ArgumentException("Query is empty. Prevent change all orders.", nameof(query));

            // Throw ex if all string is NullOrEmpty 
            if (AreNullOrEmpty(strings))
                throw new ArgumentException("All new statuses are null or empty.", nameof(strings));
        }


        /// <summary>
        /// Validates the date time.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <exception cref="ArgumentException"><paramref name="date" /> string is null or empty.</exception>
        /// <exception cref="ArgumentException"><paramref name="date" /> string is invalid.</exception>
        public static bool ValidateDateTime(string date)
        {
            if (string.IsNullOrEmpty(date))
                throw new ArgumentException("Date string is null or empty.", nameof(date));

            if (new Regex(@"^\d{4}-\d{2}-\d{2}").IsMatch(date))
            {
                DateTime dt;
                if (!DateTime.TryParse(date, out dt))
                    throw new ArgumentException("Date string is invalid.", nameof(date));
            }
            else
            {
                long dt;
                if (!long.TryParse(date, out dt))
                    throw new ArgumentException("Date string is invalid.", nameof(date));
                if (dt <= 0)
                    throw new ArgumentException("Date string is invalid.", nameof(date));
            }

            return true;
        }
    }
}