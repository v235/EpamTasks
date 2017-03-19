using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace LinqToXml
{
    public static class LinqToXml
    {
        /// <summary>
        /// Creates hierarchical data grouped by category
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation (refer to -CreateHierarchySourceFile.xml in Resources)</param>
        /// <returns>Xml representation (refer to CreateHierarchyResultFile.xml in Resources)</returns>
        public static string CreateHierarchy(string xmlRepresentation)
        {
            var xmlSourse = XElement.Parse(xmlRepresentation);
            var item = xmlSourse.Elements("Data").
                GroupBy(element => element.Element("Category").Value).
                Select(group => new XElement("Group", new XAttribute("ID", group.Key),
                    group.Select(element => new XElement("Data",
                        new XElement("Quantity", element.Element("Quantity").Value),
                        new XElement("Price", element.Element("Price").Value)))));
            xmlSourse.ReplaceAll(item);
            return xmlSourse.ToString();
        }

        /// <summary>
        /// Get list of orders numbers (where shipping state is NY) from xml representation
        /// </summary>
        /// <param name="xmlRepresentation">Orders xml representation (refer to PurchaseOrdersSourceFile.xml in Resources)</param>
        /// <returns>Concatenated orders numbers</returns>
        /// <example>
        /// 99301,99189,99110
        /// </example>
        public static string GetPurchaseOrders(string xmlRepresentation)
        {           
            XNamespace nameSpace = "http://www.adventure-works.com";
            return XElement.Parse(xmlRepresentation).Elements(nameSpace + "PurchaseOrder")
                .Where(element =>
                    element
                        .Elements(nameSpace + "Address")
                        .First(elem => elem.Attribute(nameSpace + "Type").Value == "Shipping")
                        .Element(nameSpace + "State").Value == "NY")
                .Select(element => element.Attribute(nameSpace + "PurchaseOrderNumber").Value)
                .Aggregate((x, y) => x + "," + y);
        }


        /// <summary>
        /// Reads csv representation and creates appropriate xml representation
        /// </summary>
        /// <param name="customers">Csv customers representation (refer to XmlFromCsvSourceFile.csv in Resources)</param>
        /// <returns>Xml customers representation (refer to XmlFromCsvResultFile.xml in Resources)</returns>
        public static string ReadCustomersFromCsv(string customers)
        {
            var xmlSourse = new XElement("Root");
            foreach (var item in Regex.Split(customers, "\r\n").Select(c => c.Split(',')))
            {
                xmlSourse.Add(new XElement("Customer",
                    new XAttribute("CustomerID", item[0]),
                    new XElement("CompanyName", item[1]),
                    new XElement("ContactName", item[2]),
                    new XElement("ContactTitle", item[3]),
                    new XElement("Phone", item[4]),
                    new XElement("FullAddress",
                        new XElement("Address", item[5]),
                        new XElement("City", item[6]),
                        new XElement("Region", item[7]),
                        new XElement("PostalCode", item[8]),
                        new XElement("Country", item[9])
                        )
                    ));
            }
            return xmlSourse.ToString();
        }

        /// <summary>
        /// Gets recursive concatenation of elements
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation of document with Sentence, Word and Punctuation elements. (refer to ConcatenationStringSource.xml in Resources)</param>
        /// <returns>Concatenation of all this element values.</returns>
        public static string GetConcatenationString(string xmlRepresentation)
        {
            return XElement.Parse(xmlRepresentation).Value;
        }

        /// <summary>
        /// Replaces all "customer" elements with "contact" elements with the same childs
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation with customers (refer to ReplaceCustomersWithContactsSource.xml in Resources)</param>
        /// <returns>Xml representation with contacts (refer to ReplaceCustomersWithContactsResult.xml in Resources)</returns>
        public static string ReplaceAllCustomersWithContacts(string xmlRepresentation)
        {
            var xmlSourse = XElement.Parse(xmlRepresentation);          
            var contacts =
                xmlSourse.Elements("customer")
                    .Select(customer => new XElement("contact", customer.Element("name"), customer.Element("lastname")));
            xmlSourse.ReplaceAll(contacts);
            return xmlSourse.ToString();
        }

        /// <summary>
        /// Finds all ids for channels with 2 or more subscribers and mark the "DELETE" comment
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation with channels (refer to FindAllChannelsIdsSource.xml in Resources)</param>
        /// <returns>Sequence of channels ids</returns>
        public static IEnumerable<int> FindChannelsIds(string xmlRepresentation)
        {           
            return XDocument.Parse(xmlRepresentation)
                .Element("service")
                .Elements("channel")
                .Where(c =>
                    c.Elements("subscriber").Count() >= 2 &&
                    c.DescendantNodes().OfType<XComment>().Any(comment => comment.Value == "DELETE"))
                .Select(c => Convert.ToInt32(c.Attribute("id").Value));
        }

        /// <summary>
        /// Sort customers in docement by Country and City
        /// </summary>
        /// <param name="xmlRepresentation">Customers xml representation (refer to GeneralCustomersSourceFile.xml in Resources)</param>
        /// <returns>Sorted customers representation (refer to GeneralCustomersResultFile.xml in Resources)</returns>
        public static string SortCustomers(string xmlRepresentation)
        {
            var xmlSourse = XElement.Parse(xmlRepresentation);
            var orderedCustomers = xmlSourse.Elements("Customers").
                OrderBy(customer => customer.Element("FullAddress").Element("Country").Value).
                ThenBy(customer => customer.Element("FullAddress").Element("City").Value);
            xmlSourse.ReplaceAll(orderedCustomers);
            return xmlSourse.ToString();
        }

        /// <summary>
        /// Gets XElement flatten string representation to save memory
        /// </summary>
        /// <param name="xmlRepresentation">XElement object</param>
        /// <returns>Flatten string representation</returns>
        /// <example>
        ///     <root><element>something</element></root>
        /// </example>
        public static string GetFlattenString(XElement xmlRepresentation)
        {
            return xmlRepresentation.ToString(SaveOptions.DisableFormatting);
        }
        /// <summary>
        /// Gets total value of orders by calculating products value
        /// </summary>
        /// <param name="xmlRepresentation">Orders and products xml representation (refer to GeneralOrdersFileSource.xml in Resources)</param>
        /// <returns>Total purchase value</returns>
        public static int GetOrdersValue(string xmlRepresentation)
        {
            var xmlSourse = XElement.Parse(xmlRepresentation);
            var prices = xmlSourse.Elements("products").
                Elements().Select(product => product.Attribute("Value").Value);
            return xmlSourse.Elements("Orders").
                Elements("Order").Sum(order => Convert.ToInt32(prices.ElementAt(Convert.ToInt32(order.Element("product").Value) - 1)));
        }
    }
}
