using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Omu.ValueInjecter;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Inventory.Model;
using VirtoCommerce.Domain.Pricing.Model;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogCsvImportModule.Data.Model
{
    public sealed class CsvProduct : CatalogProduct
    {
        private readonly IBlobUrlResolver _blobUrlResolver;
        public CsvProduct()
        {
            SeoInfos = new List<SeoInfo>();
            Reviews = new List<EditorialReview>();
            PropertyValues = new List<PropertyValue>();
            Images = new List<Image>();
            Assets = new List<Asset>();
            Price = new CsvPrice() { Currency = "USD" };
            Prices = new List<Price> { Price };
            Inventory = new InventoryInfo();
            EditorialReview = new EditorialReview();
            Reviews = new List<EditorialReview> { EditorialReview };
            SeoInfo = new CsvSeoInfo { ObjectType = typeof(CatalogProduct).Name };
            SeoInfos = new List<SeoInfo> { SeoInfo };
        }

        public CsvProduct(CatalogProduct product, IBlobUrlResolver blobUrlResolver, Price price, InventoryInfo inventory, SeoInfo seoInfo)
            : this()
        {
            _blobUrlResolver = blobUrlResolver;

            this.InjectFrom(product);
            PropertyValues = product.PropertyValues;
            Images = product.Images;
            Assets = product.Assets;
            Links = product.Links;
            Variations = product.Variations;
            SeoInfos = product.SeoInfos;
            Reviews = product.Reviews;
            Associations = product.Associations;
            if (price != null)
            {
                Price = price;
            }
            if (inventory != null)
            {
                Inventory = inventory;
            }
            if (seoInfo != null)
            {
                SeoInfo = seoInfo;
            }
        }
        public Price Price { get; set; }
        public InventoryInfo Inventory { get; set; }
        public EditorialReview EditorialReview { get; set; }
        public SeoInfo SeoInfo { get; set; }

        public string PriceId
        {
            get
            {
                return Price.Id;
            }
            set
            {
                Price.Id = value;
            }
        }
        public string SalePrice
        {
            get
            {
                return Price.Sale?.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Price.Sale = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                }
            }
        }

        public string ListPrice
        {
            get
            {
                return Price.List.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                Price.List = string.IsNullOrEmpty(value) ? 0 : Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            }
        }

        public string PriceMinQuantity
        {
            get
            {
                return Price.MinQuantity.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                Price.MinQuantity = Convert.ToInt32(value);
            }
        }

        public string Currency
        {
            get
            {
                return Price.Currency;
            }
            set
            {
                Price.Currency = value;
            }
        }

        public string PriceListId
        {
            get
            {
                return Price.PricelistId;
            }
            set
            {
                Price.PricelistId = value;
            }
        }

        public string FulfillmentCenterId
        {
            get
            {
                return Inventory.FulfillmentCenterId;
            }
            set
            {
                Inventory.FulfillmentCenterId = value;
            }
        }

        public string Quantity
        {
            get
            {
                return Inventory.InStockQuantity.ToString();
            }
            set
            {
                Inventory.InStockQuantity = Convert.ToInt64(value);
            }
        }

        public string PrimaryImage
        {
            get
            {
                var retVal = string.Empty;
                if (Images != null)
                {
                    var primaryImage = Images.OrderBy(x => x.SortOrder).FirstOrDefault();
                    if (primaryImage != null)
                    {
                        retVal = _blobUrlResolver != null ? _blobUrlResolver.GetAbsoluteUrl(primaryImage.Url) : primaryImage.Url;
                    }
                }
                return retVal;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    int imageOrder = 0;
                    string[] arrayOfImages = value.Split(' ');
                    foreach (string image in arrayOfImages)
                    {
                        Images.Add(new Image {
                        Url = value,
                        SortOrder = imageOrder
                    });
                        imageOrder++;
                    }
                }
            }
        }

        public string AltImage
        {
            get
            {
                var retVal = string.Empty;
                if (Images != null)
                {
                    var primaryImage = Images.OrderBy(x => x.SortOrder).Skip(1).FirstOrDefault();
                    if (primaryImage != null)
                    {
                        retVal = _blobUrlResolver != null ? _blobUrlResolver.GetAbsoluteUrl(primaryImage.Url) : primaryImage.Url;
                    }
                }
                return retVal;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Images.Add(new Image
                    {
                        Url = value,
                        SortOrder = 1
                    });
                }
            }
        }
        public string Sku
        {
            get
            {
                return Code;
            }
            set
            {
                Code = value?.Trim();
            }
        }

        public string ParentSku { get; set; }

        public string CategoryPath
        {
            get
            {
                if (Category == null)
                    return null;

                var parents = Category.Parents ?? new Category[] { };
                return string.Join("/", parents.Select(x => x.Path ?? x.Name).Concat(new[] { Category.Path ?? Category.Name }));
            }
            set
            {
                Category = new Category { Path = value };
            }
        }

        public string ReviewType
        {
            get { return EditorialReview.ReviewType; }
            set { EditorialReview.ReviewType = value; }
        }

        public string Review
        {
            get { return EditorialReview.Content; }
            set { EditorialReview.Content = value; }
        }

        public string SeoTitle
        {
            get { return SeoInfo.PageTitle; }
            set { SeoInfo.PageTitle = value; }
        }

        public string SeoUrl
        {
            get { return SeoInfo.SemanticUrl; }
            set
            {
                var slug = value;
                SeoInfo.SemanticUrl = slug.Substring(0, Math.Min(slug.Length, 240));
            }
        }

        public string SeoDescription
        {
            get { return SeoInfo.MetaDescription; }
            set { SeoInfo.MetaDescription = value; }
        }

        public string SeoLanguage
        {
            get { return SeoInfo.LanguageCode; }
            set { SeoInfo.LanguageCode = value; }
        }

        public string SeoStore
        {
            get { return SeoInfo.StoreId; }
            set { SeoInfo.StoreId = value; }
        }

        public int LineNumber { get; set; }

        /// <summary>
        /// Merge from other product, without any deletion, only update and create allowed
        /// 
        /// </summary>
        /// <param name="product"></param>
        public void MergeFrom(CatalogProduct product)
        {
            Id = product.Id;

            if (string.IsNullOrEmpty(Code))
            {
                Code = product.Code;
            }

            if (string.IsNullOrEmpty(Name))
            {
                Name = product.Name;
            }

            if (string.IsNullOrEmpty(CategoryId))
            {
                CategoryId = product.CategoryId;
            }

            if (Category == null || (Category != null && string.IsNullOrEmpty(Category.Path)))
            {
                Category = product.Category;
            }

            if (string.IsNullOrEmpty(ProductType))
            {
                ProductType = product.ProductType;
            }

            if (string.IsNullOrEmpty(Vendor))
            {
                Vendor = product.Vendor;
            }

            var imgComparer = AnonymousComparer.Create((Image x) => x.Url);
            Images = Images.Concat(product.Images).Distinct(imgComparer).ToList();

            var assetComparer = AnonymousComparer.Create((Asset x) => x.Url);
            Assets = Assets.Concat(product.Assets).Distinct(assetComparer).ToList();

            var reviewsComparer = AnonymousComparer.Create((EditorialReview x) => string.Join(":", x.ReviewType, x.LanguageCode));
            Reviews = Reviews.Concat(product.Reviews).Distinct(reviewsComparer).ToList();

            var properyValueComparer = AnonymousComparer.Create((PropertyValue x) => x.PropertyName);
            foreach (var propertyValue in PropertyValues)
            {
                var array = product.PropertyValues.Where(x => properyValueComparer.Equals(x, propertyValue)).ToArray();
                foreach (var productPropertyValue in array)
                {
                    product.PropertyValues.Remove(productPropertyValue);
                }
            }
            PropertyValues = product.PropertyValues.Concat(PropertyValues).ToList();

            //merge seo infos
            var seoComparer = AnonymousComparer.Create((SeoInfo x) => string.Join(":", x.SemanticUrl, x.LanguageCode?.ToLower(), x.StoreId));

            foreach (var seoInfo in SeoInfos.OfType<CsvSeoInfo>())
            {
                var existingSeoInfo = product.SeoInfos.FirstOrDefault(x => seoComparer.Equals(x, seoInfo));
                if (existingSeoInfo != null)
                {
                    seoInfo.MergeFrom(existingSeoInfo);
                    product.SeoInfos.Remove(existingSeoInfo);
                }
            }
            SeoInfos = SeoInfos.Where(x => !x.SemanticUrl.IsNullOrEmpty()).Concat(product.SeoInfos).ToList();
        }
    }
}
