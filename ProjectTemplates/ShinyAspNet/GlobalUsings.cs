global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
#if (shinymediator)
global using Shiny.Mediator;
#endif
#if (!efnone)
global using ShinyAspNet.Data;
#endif
#if (jwtauth)
global using ShinyAspNet.Services;
#endif
