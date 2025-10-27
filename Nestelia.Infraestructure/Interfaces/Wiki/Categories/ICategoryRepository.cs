﻿using Nestelia.Domain.Entities.Wiki.Categories;
using Nestelia.Infraestructure.Interfaces.Generic;

namespace Nestelia.Infraestructure.Interfaces.Wiki.Categories
{
    public interface ICategoryRepository: IBaseRepository<Category>
    {
    }
}
