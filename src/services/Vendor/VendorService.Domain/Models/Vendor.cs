using VendorService.Domain.Common;
using VendorService.Domain.ValueObjects;

namespace VendorService.Domain.Models
{
    public class Vendor : AuditableEntity
    {

        public string Name { get; private set; }
        public string NationalId { get; private set; } // کد ملی یا شناسه ملی
        public bool IsActive { get; private set; }
        public Address Address { get; private set; }
        public ContactInfo Contact { get; private set; }


        private Vendor() { }

        public static Vendor Create(string name, string nationalId, Address address, ContactInfo contact)
        {
            var vendor = new Vendor
            {
                Name = name,

            };

           // var createdEvent = new VendorCreatedEvent(vendor.Id, vendor.Name);
           // vendor.AddDomainEvent(createdEvent);
            return vendor;
        }
        public void UpdateInfo(string name, Address address, ContactInfo contact)
        {
            // منطق آپدیت
            Name = name;
            Address = address;
            Contact = contact;
        }
        public void Deactivate()
        {
            if (!IsPublished) return; // با توجه به مدل شما، شاید IsPublished همان IsActive باشد

            this.IsPublished = false;

            // ایجاد و افزودن رویداد هنگام غیرفعال‌سازی
            //var deactivatedEvent = new VendorDeactivatedEvent(this.Id);
            //this.AddDomainEvent(deactivatedEvent);
        }
        public void Activate()
        {
           
        }
    }
}
