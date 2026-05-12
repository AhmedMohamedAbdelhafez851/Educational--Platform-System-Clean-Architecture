//using MediatR;
//using Microsoft.Extensions.Logging;
//using OnlineExamSystem.Application.Abstraction;
//using OnlineExamSystem.Application.Caching;

//namespace OnlineExamSystem.Application.Behaviors
//{
//    public class CachingBehavior<TRequest, TResponse>
//        : IPipelineBehavior<TRequest, TResponse>
//        where TRequest : IRequest<TResponse>
//    {
//        private readonly ICacheService _cacheService;
//        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

//        public CachingBehavior(
//            ICacheService cacheService,
//            ILogger<CachingBehavior<TRequest, TResponse>> logger)
//        {
//            _cacheService = cacheService;
//            _logger = logger;
//        }

//        public async Task<TResponse> Handle(
//            TRequest request,
//            RequestHandlerDelegate<TResponse> next,
//            CancellationToken cancellationToken)
//        {
//            // ✅ ONLY CACHE QUERIES THAT IMPLEMENT ICacheableQuery
//            if (request is not ICacheableQuery cacheableQuery)
//            {
//                return await next();
//            }

//            // ✅ CHECK CACHE
//            var cachedResponse =
//                await _cacheService.GetAsync<TResponse>(
//                    cacheableQuery.CacheKey);

//            if (cachedResponse is not null)
//            {
//                _logger.LogInformation(
//                    "Cache hit for {CacheKey}",
//                    cacheableQuery.CacheKey);

//                return cachedResponse;
//            }

//            // ✅ EXECUTE HANDLER
//            var response = await next();

//            // ✅ SAVE TO CACHE
//            await _cacheService.SetAsync(
//                cacheableQuery.CacheKey,
//                response,
//                cacheableQuery.SlidingExpirationInMinutes);

//            _logger.LogInformation(
//                "Cache set for {CacheKey}",
//                cacheableQuery.CacheKey);

//            return response;
//        }
//    }
//}