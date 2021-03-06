﻿// Licensed under the GPL License, Version 3.0. See LICENSE in the git repository root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Ecwid.Test.Services
{
	[SuppressMessage("ReSharper", "ExceptionNotDocumented")]
	[SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
	public class QueryBuilderExtensionsTest
	{
		private readonly IEcwidClient _defaultClient = new EcwidClient();

		[Fact]
		public void AddOrUpdateStatusesException() => Assert.Throws<ArgumentException>(()=> _defaultClient.Orders.AddOrUpdateStatuses("", null));
		
		[Fact]
		public void AddFulfillmentStatuses()
		{
			var result =
				_defaultClient.Orders.FulfillmentStatuses("AWAITING_PROCESSING").FulfillmentStatuses("PROCESSING").GetParam("fulfillmentStatus");
			Assert.Equal(result, "AWAITING_PROCESSING,PROCESSING");
		}

		[Fact]
		public void AddTwicePaymentStatuses()
		{
			var result =
				_defaultClient.Orders.PaymentStatuses("PAID").PaymentStatuses("CANCELLED").GetParam(
					"paymentStatus");
			Assert.Equal(result, "PAID,CANCELLED");
		}

		[Fact]
		public void CouponCode()
		{
			var result = _defaultClient.Orders.CouponCode(1).GetParam("couponCode");

			Assert.Equal(1, result);
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.CouponCode(-1));
		}

		[Fact]
		public void Custom()
		{
			var result = _defaultClient.Orders.Custom("date", "test").GetParam("date");

			Assert.Equal("test", result);
		}

		[Fact]
		public void Customer()
		{
			var result = _defaultClient.Orders.Customer("test").GetParam("customer");

			Assert.Equal(result, "test");
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Customer(""));
		}

		[Fact]
		public void CustomFail()
		{
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Custom(null, new {a = 1}));
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Custom(null, null));
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Custom("", new {a = 1}));
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Custom(" ", new {a = 1}));
		}

		[Theory]
		[InlineData("2015-04-22")]
		[InlineData("2015-04-22 18:48:38")]
		[InlineData("2015-04-22 18:48:38 -0500")]
		[InlineData("1447804800")]
		public void DateTrue(string date) => Assert.NotNull(_defaultClient.Orders.CreatedFrom(date).GetParam("createdFrom"));

		[Theory]
		[InlineData("2015-00-22")]
		[InlineData("2015-00-22 18:48:38")]
		[InlineData("2015-00-22 18:48:38 -0500")]
		[InlineData("-1")]
		[InlineData("")]
		[InlineData(null)]
		[InlineData(" ")]
		[InlineData("1111111111111111111111111111111111111111111111111111111111")]
		public void DateException(string date)
			=> Assert.Throws<ArgumentException>(() => _defaultClient.Orders.CreatedFrom(date).GetParam("createdFrom"));

		[Fact]
		public void Date()
		{
			var date = new DateTime(2015, 4, 22, 18, 48, 38);
			const string check = "2015-04-22 18:48:38";

			var result = _defaultClient.Orders.CreatedFrom(date).GetParam("createdFrom");
			var result2 = _defaultClient.Orders.CreatedTo(date).GetParam("createdTo");
			var result3 = _defaultClient.Orders.UpdatedFrom(date).GetParam("updatedFrom");
			var result4 = _defaultClient.Orders.UpdatedTo(date).GetParam("updatedTo");

			var qb = _defaultClient.Orders.Created(date, date);
			var result5 = qb.GetParam("createdFrom");
			var result6 = qb.GetParam("createdTo");

			var qb2 = _defaultClient.Orders.Updated(date, date);
			var result7 = qb2.GetParam("updatedFrom");
			var result8 = qb2.GetParam("updatedTo");

			Assert.Equal(check, result);
			Assert.Equal(check, result2);
			Assert.Equal(check, result3);
			Assert.Equal(check, result4);
			Assert.Equal(check, result5);
			Assert.Equal(check, result6);
			Assert.Equal(check, result7);
			Assert.Equal(check, result8);
		}

		[Theory]
		[InlineData("2015-04-22")]
		[InlineData("2015-04-22 18:48:38")]
		[InlineData("2015-04-22 18:48:38 -0500")]
		[InlineData("1447804800")]
		public void DateString(string date)
		{
			var result = _defaultClient.Orders.CreatedFrom(date).GetParam("createdFrom");
			var result2 = _defaultClient.Orders.CreatedTo(date).GetParam("createdTo");
			var result3 = _defaultClient.Orders.UpdatedFrom(date).GetParam("updatedFrom");
			var result4 = _defaultClient.Orders.UpdatedTo(date).GetParam("updatedTo");

			var qb = _defaultClient.Orders.Created(date, date);
			var result5 = qb.GetParam("createdFrom");
			var result6 = qb.GetParam("createdTo");

			var qb2 = _defaultClient.Orders.Updated(date, date);
			var result7 = qb2.GetParam("updatedFrom");
			var result8 = qb2.GetParam("updatedTo");

			Assert.Equal(date, result);
			Assert.Equal(date, result2);
			Assert.Equal(date, result3);
			Assert.Equal(date, result4);
			Assert.Equal(date, result5);
			Assert.Equal(date, result6);
			Assert.Equal(date, result7);
			Assert.Equal(date, result8);
		}

		[Fact]
		public void FulfillmentStatuses()
		{
			var result =
				_defaultClient.Orders.FulfillmentStatuses("AWAITING_PROCESSING PROCESSING").GetParam("fulfillmentStatus");
			Assert.Equal(result, "AWAITING_PROCESSING,PROCESSING");
		}

		[Theory]
		[InlineData("PAID, DECLINE")]
		[InlineData("")]
		public void FulfillmentStatusesFail(string paid)
		{
			Assert.Throws<EcwidConfigException>(() => _defaultClient.Orders.FulfillmentStatuses(paid));
		}

		[Fact]
		public void Keywords()
		{
			var result = _defaultClient.Orders.Keywords("John").GetParam("keywords");

			Assert.Equal("John", result);
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Keywords(""));
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Keywords(null));
		}

		[Fact]
		public void LimitAndOffset()
		{
			var result = _defaultClient.Orders.Limit(12).GetParam("limit");
			var result2 = _defaultClient.Orders.Limit(120).GetParam("limit");
			var result3 = _defaultClient.Orders.Offset(100).GetParam("offset");

			Assert.Equal(12, result);
			Assert.Equal(100, result2);
			Assert.Equal(100, result3);
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Limit(-1));
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Offset(-1));
		}

		[Fact]
		public void Methods()
		{
			var result = _defaultClient.Orders.PaymentMethod("test").GetParam("paymentMethod");
			var result2 = _defaultClient.Orders.ShippingMethod("test").GetParam("shippingMethod");

			Assert.Equal("test", result);
			Assert.Equal("test", result2);
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.PaymentMethod(null));
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.PaymentMethod(""));
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.ShippingMethod(null));
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.ShippingMethod(""));
		}

		[Fact]
		public void Order()
		{
			var result = _defaultClient.Orders.Order(1).GetParam("orderNumber");
			var result2 = _defaultClient.Orders.Order("test").GetParam("vendorOrderNumber");

			Assert.Equal(1, result);
			Assert.Equal("test", result2);
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Order(-1));
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Order(""));
		}

		[Fact]
		public void PaymentStatuses()
		{
			var result = _defaultClient.Orders.PaymentStatuses("PAID, CANCELLED").GetParam("paymentStatus");
			Assert.Equal(result, "PAID,CANCELLED");
		}

		[Theory]
		[InlineData("PAID, DECLINE")]
		[InlineData("")]
		public void PaymentStatusesFail(string paid)
		{
			Assert.Throws<EcwidConfigException>(() => _defaultClient.Orders.PaymentStatuses(paid));
		}

		[Fact]
		public void Totals()
		{
			var qb = _defaultClient.Orders.Totals(1, 1);
			var result = qb.GetParam("totalFrom");
			var result2 = qb.GetParam("totalTo");
			var result3 = _defaultClient.Orders.TotalFrom(1).GetParam("totalFrom");
			var result4 = _defaultClient.Orders.TotalTo(1).GetParam("totalTo");

			Assert.Equal(1.0, result);
			Assert.Equal(1.0, result2);
			Assert.Equal(1.0, result3);
			Assert.Equal(1.0, result4);
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Totals(-1, -1));
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Totals(-1, 1));
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.Totals(1, -1));
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.TotalFrom(-1));
			Assert.Throws<ArgumentException>(() => _defaultClient.Orders.TotalTo(-1));
		}
	}
}