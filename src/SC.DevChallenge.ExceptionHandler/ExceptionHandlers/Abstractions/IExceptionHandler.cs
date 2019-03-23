﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SC.DevChallenge.ExceptionHandler
{
    public interface IExceptionHandler
    {
        Task HandleException(Exception exception, HttpContext context);
    }
}
