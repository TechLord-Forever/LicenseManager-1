﻿using System.Web.Mvc;
using LicenseManager.Infrastructure.Exceptions;

namespace LicenseManager.Infrastructure.Attributes
{
    internal sealed class ReadOnlyModeErrorFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ReadOnlyModeException)
            {
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.TrySkipIisCustomErrors = true;
                context.HttpContext.Response.StatusCode = 503;

                context.Result = new ViewResult
                {
                    ViewName = "~/Errors/ReadOnlyMode.cshtml",
                };

                context.ExceptionHandled = true;
            }
        }
    }
}