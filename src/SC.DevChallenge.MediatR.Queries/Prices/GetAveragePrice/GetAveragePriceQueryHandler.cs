﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SC.DevChallenge.DataAccess.Abstractions.Entities;
using SC.DevChallenge.DataAccess.Abstractions.Repositories;
using SC.DevChallenge.Domain.Date.DateTimeConverter;
using SC.DevChallenge.Dto;
using SC.DevChallenge.MediatR.Core.HandlerResults.Abstractions;
using SC.DevChallenge.MediatR.Queries.Prices.GetAverage;
using SC.DevChallenge.MediatR.Queries.Prices.GetAveragePrice.Specifications;

namespace SC.DevChallenge.MediatR.Queries.Prices.GetAveragePrice
{
    public class GetAveragePriceQueryHandler : RequestHandlerBase<GetAveragePriceQuery, AveragePriceDto>
    {
        private readonly IPriceRepository priceRepository;
        private readonly IGetAveragePriceSpecification getAveragePriceSpecification;
        private readonly IDateTimeConverter dateTimeConverter;

        public GetAveragePriceQueryHandler(
            IPriceRepository priceRepository,
            IGetAveragePriceSpecification getAveragePriceSpecification,
            IDateTimeConverter dateTimeConverter)
        {
            this.priceRepository = priceRepository;
            this.getAveragePriceSpecification = getAveragePriceSpecification;
            this.dateTimeConverter = dateTimeConverter;
        }

        public async override Task<IHandlerResult<AveragePriceDto>> Handle(
            GetAveragePriceQuery request,
            CancellationToken cancellationToken)
        {
            var timeslot = dateTimeConverter.DateTimeToTimeSlot(request.Date);
            var startDate = dateTimeConverter.GetTimeSlotStartDate(timeslot);

            var filter = getAveragePriceSpecification.ToExpression(request.Portfolio, request.Owner, request.Instrument, timeslot);

            var prices = await priceRepository.GetAllAsync(filter, p => p.Value);

            if (prices.Any())
            {
                var result = AveragePriceDto.Create(startDate, prices.Average());
                return Data(result);
            }

            return NotFound();
        }
    }
}