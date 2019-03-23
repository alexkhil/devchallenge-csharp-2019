﻿using System;
using System.Net;

namespace SC.DevChallenge.ExceptionHandler.ExceptionHandlers
{
    public class DefaultExceptionHandler : BaseExceptionHandler, IExceptionHandler<Exception>
    {
        private const string ErrorMessage = "Some unexpected error occurred.";

        protected override ErrorResponse CreateErrorMessage(Exception exception)
        {
            return new ErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage);
        }
    }
}