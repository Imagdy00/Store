﻿using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories;
public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    private readonly StoreDbContext _context;

    public GenericRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool trackChanges = false)
    {

        if(typeof(TEntity) == typeof(Product))
        {
            return trackChanges ?
            await _context.Products.Include(p => p.ProductBrand).Include(p => p.ProductType).ToListAsync() as IEnumerable<TEntity>
            : await _context.Products.Include(p => p.ProductBrand).Include(p => p.ProductType).AsNoTracking().ToListAsync() as IEnumerable<TEntity>;
        }
        return trackChanges?
            await _context.Set<TEntity>().ToListAsync()
            :await _context.Set<TEntity>().AsNoTracking().ToListAsync();

        //if(trachChanges) return await _context.Set<TEntity>().ToListAsync();

        //return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
    }

    public async Task<TEntity?> GetAsync(int Id)
    {
        if (typeof(TEntity) == typeof(Product))
        {
            return await _context.Products.Include(p => p.ProductBrand).Include(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == Id as int?) as TEntity;

        }
        return await _context.Set<TEntity>().FindAsync(Id);
    }


    public async Task AddAsync(TEntity entity)
    {
        await _context.AddAsync(entity);
    }


    public async void Update(TEntity entity)
    {
         _context.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _context.Remove(entity);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> spec, bool trackChanges = false)
    {
        return await ApplySpecifications(spec).ToListAsync();
    }

    public async Task<TEntity?> GetAsync(ISpecifications<TEntity, TKey> spec)
    {
        return await ApplySpecifications(spec).FirstOrDefaultAsync();
    }

    public async Task<int> CountAsync(ISpecifications<TEntity, TKey> spec)
    {
        return await ApplySpecifications(spec).CountAsync();
    }

    private IQueryable<TEntity> ApplySpecifications(ISpecifications<TEntity , TKey> spec)
    {
        return SpecificationEvaluator.GetQuery(_context.Set<TEntity>(), spec);
    }

    
}
