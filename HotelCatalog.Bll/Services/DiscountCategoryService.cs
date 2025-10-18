using AutoMapper;
using Microsoft.Extensions.Logging;
using HotelCatalog.Bll.DTOs;
using HotelCatalog.Bll.Interfaces;
using HotelCatalog.Dal.Interfaces;
using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Bll.Services
{
    public class DiscountCategoryService : IDiscountCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DiscountCategoryService> _logger;

        public DiscountCategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DiscountCategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DiscountCategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var discount = await _unitOfWork.DiscountCategories.GetByIdAsync(id, cancellationToken);
            return discount != null ? _mapper.Map<DiscountCategoryDto>(discount) : null;
        }

        public async Task<IEnumerable<DiscountCategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var discounts = await _unitOfWork.DiscountCategories.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<DiscountCategoryDto>>(discounts);
        }

        public async Task<IEnumerable<DiscountCategoryDto>> GetDiscountsAbovePercentAsync(decimal percent, CancellationToken cancellationToken = default)
        {
            if (percent < 0 || percent > 100)
            {
                throw new ArgumentException("Discount percent must be between 0 and 100");
            }

            var discounts = await _unitOfWork.DiscountCategories.GetDiscountsAbovePercentAsync(percent, cancellationToken);
            return _mapper.Map<IEnumerable<DiscountCategoryDto>>(discounts);
        }

        public async Task<DiscountCategoryDto> CreateAsync(CreateDiscountCategoryDto dto, CancellationToken cancellationToken = default)
        {
            var existing = await _unitOfWork.DiscountCategories.GetByNameAsync(dto.Name, cancellationToken);
            if (existing != null)
            {
                throw new InvalidOperationException($"Discount category with name '{dto.Name}' already exists");
            }

            var discount = _mapper.Map<DiscountCategory>(dto);

            await _unitOfWork.DiscountCategories.AddAsync(discount, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Discount category created with ID: {DiscountId}", discount.DiscountId);

            return _mapper.Map<DiscountCategoryDto>(discount);
        }

        public async Task<DiscountCategoryDto> UpdateAsync(UpdateDiscountCategoryDto dto, CancellationToken cancellationToken = default)
        {
            var discount = await _unitOfWork.DiscountCategories.GetByIdAsync(dto.DiscountId, cancellationToken);

            if (discount == null)
            {
                throw new KeyNotFoundException($"Discount category with ID {dto.DiscountId} not found");
            }

            if (discount.Name != dto.Name)
            {
                var existing = await _unitOfWork.DiscountCategories.GetByNameAsync(dto.Name, cancellationToken);
                if (existing != null)
                {
                    throw new InvalidOperationException($"Discount category with name '{dto.Name}' already exists");
                }
            }

            _mapper.Map(dto, discount);
            _unitOfWork.DiscountCategories.Update(discount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Discount category updated with ID: {DiscountId}", discount.DiscountId);

            return _mapper.Map<DiscountCategoryDto>(discount);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var discount = await _unitOfWork.DiscountCategories.GetByIdAsync(id, cancellationToken);

            if (discount == null)
            {
                return false;
            }

            _unitOfWork.DiscountCategories.Delete(discount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Discount category deleted with ID: {DiscountId}", id);

            return true;
        }
    }
}