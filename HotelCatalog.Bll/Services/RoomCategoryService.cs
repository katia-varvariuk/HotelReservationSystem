﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using HotelCatalog.Bll.DTOs;
using HotelCatalog.Bll.Interfaces;
using HotelCatalog.Dal.Interfaces;
using HotelCatalog.Domain.Entities;

namespace HotelCatalog.Bll.Services
{
    public class RoomCategoryService : IRoomCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomCategoryService> _logger;

        public RoomCategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoomCategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RoomCategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.RoomCategories.GetByIdAsync(id, cancellationToken);
            return category != null ? _mapper.Map<RoomCategoryDto>(category) : null;
        }

        public async Task<RoomCategoryDto?> GetByIdWithServicesAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.RoomCategories.GetCategoryByIdAsync(id, cancellationToken);

            if (category == null)
            {
                return null;
            }

            await _unitOfWork.RoomCategories.LoadServicesAsync(category, cancellationToken);

            return _mapper.Map<RoomCategoryDto>(category);
        }

        public async Task<IEnumerable<RoomCategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _unitOfWork.RoomCategories.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<RoomCategoryDto>>(categories);
        }

        public async Task<IEnumerable<RoomCategoryDto>> GetCategoriesWithServiceCountAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _unitOfWork.RoomCategories.GetCategoriesWithServiceCountAsync(cancellationToken);
            return _mapper.Map<IEnumerable<RoomCategoryDto>>(categories);
        }

        public async Task<IEnumerable<ServiceDto>> GetServicesByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var services = await _unitOfWork.RoomCategories.GetServicesByCategoryAsync(categoryId, cancellationToken);
            return _mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        public async Task<RoomCategoryDto> CreateAsync(CreateRoomCategoryDto dto, CancellationToken cancellationToken = default)
        {
            var category = _mapper.Map<RoomCategory>(dto);

            await _unitOfWork.RoomCategories.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Room category created with ID: {CategoryId}", category.CategoryId);

            return _mapper.Map<RoomCategoryDto>(category);
        }

        public async Task<RoomCategoryDto> UpdateAsync(UpdateRoomCategoryDto dto, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.RoomCategories.GetByIdAsync(dto.CategoryId, cancellationToken);

            if (category == null)
            {
                throw new KeyNotFoundException($"Room category with ID {dto.CategoryId} not found");
            }

            _mapper.Map(dto, category);
            _unitOfWork.RoomCategories.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Room category updated with ID: {CategoryId}", category.CategoryId);

            return _mapper.Map<RoomCategoryDto>(category);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.RoomCategories.GetByIdAsync(id, cancellationToken);

            if (category == null)
            {
                return false;
            }

            _unitOfWork.RoomCategories.Delete(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Room category deleted with ID: {CategoryId}", id);

            return true;
        }

        public async Task<bool> AssignServiceToCategoryAsync(AssignServiceToCategoryDto dto, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.RoomCategories.GetByIdAsync(dto.CategoryId, cancellationToken);
            var service = await _unitOfWork.Services.GetByIdAsync(dto.ServiceId, cancellationToken);

            if (category == null || service == null)
            {
                return false;
            }

            var roomService = new RoomService
            {
                CategoryId = dto.CategoryId,
                ServiceId = dto.ServiceId
            };

            await _unitOfWork.Repository<RoomService>().AddAsync(roomService, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Service {ServiceId} assigned to category {CategoryId}", dto.ServiceId, dto.CategoryId);

            return true;
        }

        public async Task<bool> RemoveServiceFromCategoryAsync(int categoryId, int serviceId, CancellationToken cancellationToken = default)
        {
            var roomService = await _unitOfWork.Repository<RoomService>()
                .FirstOrDefaultAsync(rs => rs.CategoryId == categoryId && rs.ServiceId == serviceId, cancellationToken);

            if (roomService == null)
            {
                return false;
            }

            _unitOfWork.Repository<RoomService>().Delete(roomService);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Service {ServiceId} removed from category {CategoryId}", serviceId, categoryId);

            return true;
        }
    }
}